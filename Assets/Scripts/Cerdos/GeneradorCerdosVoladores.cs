using System;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorCerdosVoladores : MonoBehaviour
{
    [Serializable]
    public class TipoCerdoGenerable
    {
        [Tooltip("Prefab del tipo de cerdo que puede aparecer.")]
        public CerdoVolador prefabCerdo;

        [Tooltip("Peso de aparición. Mientras más alto, más probable es que aparezca.")]
        [Min(0f)] public float pesoAparicion = 1f;
    }
    public List<CerdoVolador> cerdosVivos;

    [Header("Tipos de cerdos disponibles")]
    [SerializeField] private TipoCerdoGenerable[] tiposDeCerdos;

    [Header("Puntos de aparición")]
    [SerializeField] private Transform[] puntosDeAparicion;

    [Header("Tiempo entre apariciones")]
    [SerializeField] private float tiempoMinimoEntreApariciones = 1f;
    [SerializeField] private float tiempoMaximoEntreApariciones = 2.5f;

    [Header("Limite de generaciones")]
    [SerializeField] private int spawns = 0;
    [SerializeField] private int spawnLimit = 20;

    private float tiempoParaProximaAparicion;
    private bool generacionTerminada = false;

    private void Start()
    {
        ProgramarProximaAparicion();
    }

    private void Update()
    {
        tiempoParaProximaAparicion -= Time.deltaTime;

        if (tiempoParaProximaAparicion <= 0f && spawns < spawnLimit)
        {
            CrearCerdoVolador();
            ProgramarProximaAparicion();
        }
        if (spawns >= spawnLimit && cerdosVivos.Count == 0)
        {
            Debug.Log("Fin del juego");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.NotificarGeneradorTerminado();
            }
        }
        cerdosVivos.RemoveAll(CerdoVolador => CerdoVolador == null);
    }

    private void CrearCerdoVolador()
    {
        CerdoVolador prefabElegido = ElegirPrefabCerdo();

        if (prefabElegido == null)
        {
            Debug.LogWarning("No hay ningún prefab de cerdo válido asignado en el generador.");
            return;
        }

        if (puntosDeAparicion == null || puntosDeAparicion.Length == 0)
        {
            Debug.LogWarning("Falta asignar los puntos de aparición.");
            return;
        }

        int indiceAleatorio = UnityEngine.Random.Range(0, puntosDeAparicion.Length);
        Transform puntoElegido = puntosDeAparicion[indiceAleatorio];

        Vector3 posicionAparicion = puntoElegido.position;
        posicionAparicion.z = 0f;

        cerdosVivos.Add(Instantiate(
            prefabElegido,
            posicionAparicion,
            Quaternion.identity
        ));
        spawns++;
    }
    private CerdoVolador ElegirPrefabCerdo()
    {
        if (tiposDeCerdos == null || tiposDeCerdos.Length == 0)
        {
            return null;
        }

        float pesoTotal = 0f;

        for (int i = 0; i < tiposDeCerdos.Length; i++)
        {
            if (tiposDeCerdos[i] != null &&
                tiposDeCerdos[i].prefabCerdo != null &&
                tiposDeCerdos[i].pesoAparicion > 0f)
            {
                pesoTotal += tiposDeCerdos[i].pesoAparicion;
            }
        }

        if (pesoTotal <= 0f)
        {
            return null;
        }

        float resultadoAleatorio = UnityEngine.Random.Range(0f, pesoTotal);
        float pesoAcumulado = 0f;

        for (int i = 0; i < tiposDeCerdos.Length; i++)
        {
            if (tiposDeCerdos[i] == null ||
                tiposDeCerdos[i].prefabCerdo == null ||
                tiposDeCerdos[i].pesoAparicion <= 0f)
            {
                continue;
            }

            pesoAcumulado += tiposDeCerdos[i].pesoAparicion;

            if (resultadoAleatorio <= pesoAcumulado)
            {
                return tiposDeCerdos[i].prefabCerdo;
            }
        }

        return null;
    }
    private void ProgramarProximaAparicion()
    {
        tiempoParaProximaAparicion = UnityEngine.Random.Range(
            tiempoMinimoEntreApariciones,
            tiempoMaximoEntreApariciones
        );
    }
    public int GetMaximoCerdos() => spawnLimit;
    public int GetCerdosGenerados() => spawns;
    public void DetenerGeneracion()
    {
        generacionTerminada = true;
        this.enabled = false;
    }
}