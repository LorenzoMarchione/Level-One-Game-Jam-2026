using System;
using System.Collections;
using UnityEngine;

public class CerdoVolador : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 1;

    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2.5f;

    [Tooltip("Empuje general hacia arriba. Más alto = escapan más rápido hacia arriba.")]
    [SerializeField] private float tendenciaHaciaArriba = 0.35f;

    [Tooltip("Intensidad del movimiento irregular en todas las direcciones.")]
    [SerializeField] private float intensidadMovimientoAleatorio = 2.5f;

    [Tooltip("Sacudida nerviosa extra, similar al movimiento de una mosca.")]
    [SerializeField] private float intensidadSacudida = 1.2f;

    [Tooltip("Qué tan rápido cambia el ruido del movimiento.")]
    [SerializeField] private float velocidadVariacionAleatoria = 5f;

    [Tooltip("Qué tan rápido gira el cerdo hacia la nueva dirección.")]
    [SerializeField] private float suavizadoCambioDireccion = 8f;

    [Header("Puntos")]
    [SerializeField] private int puntosMinimos = 10;
    [SerializeField] private int puntosMaximos = 30;

    [Header("Recompensa")]
    [SerializeField] private GameObject prefabRecompensa;
    [SerializeField] private float probabilidadSoltarRecompensa = 0.1f;

    [Header("Límites")]
    [SerializeField] private float margenSuperiorEscape = 1f;
    [SerializeField] private float margenInferiorPermitido = 0.5f;

    public GeneradorCerdosVoladores generador;

    // EVENTO PARA OTROS SCRIPTS:
    // Se ejecuta cuando el cerdo es derribado.
    public event Action<CerdoVolador, int> AlSerDerribado;

    // EVENTO PARA OTROS SCRIPTS:
    // Se ejecuta cuando el cerdo escapa por arriba de la pantalla.
    public event Action<CerdoVolador> AlEscapar;

    private int vidaActual;

    private Vector2 direccionActual;

    private Camera camaraPrincipal;

    private float limiteIzquierdo;
    private float limiteDerecho;
    private float limiteInferior;
    private float limiteSuperior;

    public bool yaFinalizo;

    private float semillaAleatoriaX;
    private float semillaAleatoriaY;

    private void Start()
    {
        vidaActual = vidaMaxima;
        camaraPrincipal = Camera.main;

        semillaAleatoriaX = UnityEngine.Random.Range(0f, 1000f);
        semillaAleatoriaY = UnityEngine.Random.Range(0f, 1000f);

        CalcularLimitesPantalla();

        direccionActual = Vector2.up;
    }

    private void Update()
    {
        if (yaFinalizo)
        {
            return;
        }

        ActualizarDireccionAleatoria();
        Mover();
        ControlarLimitesPantalla();
    }

    private void ActualizarDireccionAleatoria()
    {
        float tiempo = Time.time * velocidadVariacionAleatoria;

        float movimientoHorizontal = Mathf.PerlinNoise(semillaAleatoriaX, tiempo) * 2f - 1f;
        float movimientoVertical = Mathf.PerlinNoise(semillaAleatoriaY, tiempo) * 2f - 1f;

        Vector2 direccionPorRuido = new Vector2(
            movimientoHorizontal,
            movimientoVertical
        ) * intensidadMovimientoAleatorio;

        Vector2 sacudidaAleatoria = UnityEngine.Random.insideUnitCircle * intensidadSacudida;

        Vector2 empujeHaciaArriba = Vector2.up * tendenciaHaciaArriba;

        Vector2 direccionCombinada = direccionPorRuido + sacudidaAleatoria + empujeHaciaArriba;

        if (direccionCombinada.sqrMagnitude < 0.01f)
        {
            direccionCombinada = Vector2.up;
        }

        Vector2 direccionDeseada = direccionCombinada.normalized;

        direccionActual = Vector2.Lerp(
            direccionActual,
            direccionDeseada,
            suavizadoCambioDireccion * Time.deltaTime
        ).normalized;
    }

    private void Mover()
    {
        transform.position += (Vector3)(direccionActual * velocidad * Time.deltaTime);
    }

    private void ControlarLimitesPantalla()
    {
        Vector3 posicionActual = transform.position;

        if (posicionActual.x < limiteIzquierdo)
        {
            posicionActual.x = limiteIzquierdo;
            direccionActual.x = Mathf.Abs(direccionActual.x);
        }

        if (posicionActual.x > limiteDerecho)
        {
            posicionActual.x = limiteDerecho;
            direccionActual.x = -Mathf.Abs(direccionActual.x);
        }

        if (posicionActual.y < limiteInferior)
        {
            posicionActual.y = limiteInferior;
            direccionActual.y = Mathf.Abs(direccionActual.y);
        }

        transform.position = posicionActual;

        if (transform.position.y > limiteSuperior)
        {
            Escapar();
        }
    }

    private void CalcularLimitesPantalla()
    {
        if (camaraPrincipal == null)
        {
            Debug.LogWarning("No se encontró la cámara principal. Revisar que tenga el tag MainCamera.");
            return;
        }

        float distanciaACamara = Mathf.Abs(camaraPrincipal.transform.position.z);

        Vector3 esquinaInferiorIzquierda = camaraPrincipal.ViewportToWorldPoint(
            new Vector3(0f, 0f, distanciaACamara)
        );

        Vector3 esquinaSuperiorDerecha = camaraPrincipal.ViewportToWorldPoint(
            new Vector3(1f, 1f, distanciaACamara)
        );

        limiteIzquierdo = esquinaInferiorIzquierda.x;
        limiteDerecho = esquinaSuperiorDerecha.x;
        limiteInferior = esquinaInferiorIzquierda.y - margenInferiorPermitido;
        limiteSuperior = esquinaSuperiorDerecha.y + margenSuperiorEscape;
    }

    // MÉTODO PARA CONECTAR CON EL SCRIPT DE DISPARO:
    // El script que detecte el click sobre el cerdo debe llamar a este método.
    public void RecibirDisparo(int dano)
    {
        if (yaFinalizo)
        {
            return;
        }

        vidaActual -= dano;

        if (vidaActual <= 0)
        {
            Derribar();
        }
        else
        {
            StartCoroutine(ReaccionarAlDisparo());
        }
    }

    private void Derribar()
    {
        yaFinalizo = true;

        int puntosOtorgados = UnityEngine.Random.Range(puntosMinimos, puntosMaximos + 1);

        IntentarSoltarRecompensa();

        // AVISO PARA OTROS SCRIPTS:
        // Acá se informa que el cerdo fue derribado.
        AlSerDerribado?.Invoke(this, puntosOtorgados);
        Destroy(gameObject);
    }

    private void Escapar()
    {
        yaFinalizo = true;

        // AVISO PARA OTROS SCRIPTS:
        // Acá se informa que el cerdo escapó.
        AlEscapar?.Invoke(this);
        Destroy(gameObject);
    }

    private void IntentarSoltarRecompensa()
    {
        if (prefabRecompensa == null)
        {
            return;
        }

        float resultadoAleatorio = UnityEngine.Random.value;

        if (resultadoAleatorio <= probabilidadSoltarRecompensa)
        {
            Instantiate(prefabRecompensa, transform.position, Quaternion.identity);
        }
    }

    private IEnumerator ReaccionarAlDisparo()
    {
        Vector3 escalaOriginal = transform.localScale;

        transform.localScale = escalaOriginal * 1.15f;

        yield return new WaitForSeconds(0.08f);

        transform.localScale = escalaOriginal;
    }

}