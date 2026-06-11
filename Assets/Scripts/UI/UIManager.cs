using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    [Header("Textos UI")]
    [SerializeField] private TextMeshProUGUI textoCazados;  
    [SerializeField] private TextMeshProUGUI textoEscapados;
    [SerializeField] private TextMeshProUGUI textoPuntuacion;
    [SerializeField] private TextMeshProUGUI textoMunicion;
    
    [Header("Efectos Visuales")]
    [SerializeField] private Color colorNormalPuntuacion = Color.white;
    [SerializeField] private Color colorAlSumarPuntos = Color.green;
    [SerializeField] private float duracionEfectoColor = 0.3f;
    
    [Header("Nombres de Escenas")]
    [SerializeField] private string WinScene = "WinScene";
    [SerializeField] private string LoseScene = "LoseScene";
    
    private Coroutine efectoPuntuacionActual;
    private Player playerReferencia;

    private Vector3 escalaOriginal;
    
    private void Start()
    {
        InicializarTextos();
        SuscribirseEventos();
        
        playerReferencia = FindFirstObjectByType<Player>();
        if (playerReferencia != null)
        {
            ActualizarTextoMunicion(playerReferencia.currentAmmo, playerReferencia.stats.maxAmmo);
        }

        escalaOriginal = textoPuntuacion.transform.localScale;
    }
    
    private void Update()
    {
        if (playerReferencia != null && textoMunicion != null)
        {
            ActualizarTextoMunicion(playerReferencia.currentAmmo, playerReferencia.stats.maxAmmo);
        }
    }
    
    private void InicializarTextos()
    {
        if (GameManager.Instance != null)
        {
            ActualizarTextoCazados(
                GameManager.Instance.GetCerdosCazados(), 
                GameManager.Instance.GetCerdosRequeridos()
            );
            
            ActualizarTextoEscapados(
                GameManager.Instance.GetCerdosEscapados(), 
                GameManager.Instance.GetEscapesPermitidos()
            );
        }
        else
        {
            if (textoCazados != null) textoCazados.text = "Hunted: 0/20";
            if (textoEscapados != null) textoEscapados.text = "Escaped: 0/5";
        }
        
        if (ScoreManager.Instance != null)
        {
            ActualizarTextoPuntuacion(ScoreManager.Instance.GetPuntuacion());
        }
        else
        {
            if (textoPuntuacion != null) textoPuntuacion.text = "Points: 0";
        }
        
        if (playerReferencia != null && textoMunicion != null)
        {
            ActualizarTextoMunicion(playerReferencia.currentAmmo, playerReferencia.stats.maxAmmo);
        }
    }
    
    private void SuscribirseEventos()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnContadorCazadosActualizado += ActualizarTextoCazados;
            GameManager.Instance.OnContadorEscapadosActualizado += ActualizarTextoEscapados;
            GameManager.Instance.OnJuegoTerminado += MostrarPantallaFinJuego;
        }
        
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnPuntuacionCambiada += ActualizarTextoPuntuacion;
            ScoreManager.Instance.OnPuntosAgregadosConEfecto += ActivarEfectoPuntuacion;
        }
    }
    
    private void ActualizarTextoCazados(int actual, int requerido)
    {
        if (textoCazados != null)
        {
            textoCazados.text = $"Hunted: {actual}/{requerido}";
        }
    }
    
    private void ActualizarTextoEscapados(int actual, int maximo)
    {
        if (textoEscapados != null)
        {
            textoEscapados.text = $"Escaped: {actual}/{maximo}";
        }
    }
    
    private void ActualizarTextoPuntuacion(int puntos)
    {
        if (textoPuntuacion != null)
        {
            textoPuntuacion.text = $"Points: {puntos}";
        }
    }
    
    private void ActualizarTextoMunicion(int actual, int maximo)
    {
        if (textoMunicion != null)
        {
            textoMunicion.text = $"Ammo: {actual}/{maximo}";
        }
    }
    
    private void ActivarEfectoPuntuacion(int puntosSumados)
    {
        if (textoPuntuacion == null) return;
        
        if (efectoPuntuacionActual != null)
            StopCoroutine(efectoPuntuacionActual);
        
        efectoPuntuacionActual = StartCoroutine(EfectoSumarPuntos(puntosSumados));
    }
    
    private IEnumerator EfectoSumarPuntos(int puntos)
    {
        textoPuntuacion.color = colorAlSumarPuntos;
        textoPuntuacion.text = $"+{puntos}!";
        
        textoPuntuacion.transform.localScale = escalaOriginal * 1.3f;
        
        yield return new WaitForSeconds(duracionEfectoColor);
        
        textoPuntuacion.color = colorNormalPuntuacion;
        textoPuntuacion.transform.localScale = escalaOriginal;
        
        if (ScoreManager.Instance != null)
        {
            ActualizarTextoPuntuacion(ScoreManager.Instance.GetPuntuacion());
        }
    }
    
    private void MostrarPantallaFinJuego(bool victoria)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        if (victoria)
        {
            SceneManager.LoadScene(WinScene);
        }
        else
        {
            SceneManager.LoadScene(LoseScene);
        }
    }
    
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnContadorCazadosActualizado -= ActualizarTextoCazados;
            GameManager.Instance.OnContadorEscapadosActualizado -= ActualizarTextoEscapados;
            GameManager.Instance.OnJuegoTerminado -= MostrarPantallaFinJuego;
        }
        
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnPuntuacionCambiada -= ActualizarTextoPuntuacion;
            ScoreManager.Instance.OnPuntosAgregadosConEfecto -= ActivarEfectoPuntuacion;
        }
    }
}