using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Condiciones de Victoria/Derrota")]
    [SerializeField] private int cerdosRequeridos = 20;      
    [SerializeField] private int escapesPermitidos = 5;      
    
    [Header("Estados")]
    [SerializeField] private bool juegoTerminado = false;
    [SerializeField] private bool juegoPausado = false;

    // Contadores internos
    private int cerdosCazados = 0;
    private int cerdosEscapados = 0;
    public event Action<int, int> OnContadorCazadosActualizado;     
    public event Action<int, int> OnContadorEscapadosActualizado;     
    public event Action<bool> OnJuegoTerminado;                 
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        OnContadorCazadosActualizado?.Invoke(cerdosCazados, cerdosRequeridos);
        OnContadorEscapadosActualizado?.Invoke(cerdosEscapados, escapesPermitidos);
    }
    
    public void RegistrarCerdoCazado()
    {
        if (juegoTerminado) return;
        
        cerdosCazados++;
        OnContadorCazadosActualizado?.Invoke(cerdosCazados, cerdosRequeridos);
        
        Debug.Log($"Cerdo cazado! {cerdosCazados}/{cerdosRequeridos}");
        
        if (cerdosCazados >= cerdosRequeridos)
        {
            Victoria();
        }
    }
    
    public void RegistrarCerdoEscapado()
    {
        if (juegoTerminado) return;
        
        cerdosEscapados++;
        OnContadorEscapadosActualizado?.Invoke(cerdosEscapados, escapesPermitidos);
        
        Debug.Log($"Cerdo escapado! {cerdosEscapados}/{escapesPermitidos}");
        
        if (cerdosEscapados >= escapesPermitidos)
        {
            Derrota();
        }
    }
    
    private void Victoria()
    {
        if (juegoTerminado) return;
        
        juegoTerminado = true;
        OnJuegoTerminado?.Invoke(true);
        
        DetenerGenerador();
        
        SceneManager.LoadScene("WinScene");
    }

    private void Derrota()
    {
        if (juegoTerminado) return;
        
        juegoTerminado = true;
        
        OnJuegoTerminado?.Invoke(false);
        
        DetenerGenerador();
        
        SceneManager.LoadScene("LoseScene");
    }
    
    private void DetenerGenerador()
    {
        GeneradorCerdosVoladores generador = FindObjectOfType<GeneradorCerdosVoladores>();
        if (generador != null)
        {
            generador.enabled = false;
        }
    }
    
    public bool JuegoTerminado() => juegoTerminado;
    public int GetCerdosCazados() => cerdosCazados;
    public int GetCerdosEscapados() => cerdosEscapados;
    public int GetCerdosRequeridos() => cerdosRequeridos;
    public int GetEscapesPermitidos() => escapesPermitidos;
}