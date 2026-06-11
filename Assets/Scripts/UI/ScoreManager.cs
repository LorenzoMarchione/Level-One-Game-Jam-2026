using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    [Header("Configuración")]
    [SerializeField] private PlayerStats playerStats;
    
    private int puntuacionActual = 0;
    
    // Eventos
    public event Action<int> OnPuntuacionCambiada;
    public event Action<int> OnPuntosAgregadosConEfecto;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        puntuacionActual = 0;
        OnPuntuacionCambiada?.Invoke(puntuacionActual);
    }
    
    public void AgregarPuntos(int puntosBase)
    {
        int puntosFinales = puntosBase;
        
        if (playerStats != null)
        {
            puntosFinales = Mathf.RoundToInt(puntosBase * playerStats.scoreMultiplier);
        }
        
        puntuacionActual += puntosFinales;
        
        OnPuntuacionCambiada?.Invoke(puntuacionActual);
        
        OnPuntosAgregadosConEfecto?.Invoke(puntosFinales);
        
        Debug.Log($"+{puntosFinales} Points  (Total: {puntuacionActual})");
    }
    
    public int GetPuntuacion() => puntuacionActual;
    public void ReiniciarPuntuacion() => puntuacionActual = 0;
}