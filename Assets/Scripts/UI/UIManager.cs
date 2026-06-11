using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    [Header("Textos UI")]
    [SerializeField] private TextMeshProUGUI textoCazados;  
    [SerializeField] private TextMeshProUGUI textoEscapados;
    [SerializeField] private TextMeshProUGUI textoPuntuacion;
    [SerializeField] private TextMeshProUGUI textoMunicion;
    
    [Header("Efectos Visuales")]
    [SerializeField] private Color colorNormalPuntuacion = Color.black;
    [SerializeField] private Color colorAlSumarPuntos = Color.green;
    [SerializeField] private float duracionEfectoColor = 0.3f;
    
    [Header("Nombres de Escenas")]
    [SerializeField] private string WinScene = "WinScene";
    [SerializeField] private string LoseScene = "LoseScene";
    
    [Header("Prefabs de Cerdos para el HUD")]
    [SerializeField] private GameObject prefabCerdoNormal;
    [SerializeField] private GameObject prefabCerdoCazado;
    [SerializeField] private GameObject prefabCerdoEscapado;
    
    [Header("Contenedor")]
    [SerializeField] private Transform contenedorCerdos;   
    
    private List<GameObject> ranurasCerdos = new List<GameObject>();
    private int totalCerdos = 0;
    private int cerdosProcesados = 0; 
    
    private Coroutine efectoPuntuacionActual;
    private Player playerReferencia;
    private bool efectoEnProgreso = false;
    private Vector3 escalaOriginal;

    private void Start()
    {
        InicializarTextos();
        SuscribirseEventos();
        
        int cerdosRequeridos = GameManager.Instance != null ? 
            GameManager.Instance.GetCerdosRequeridos() : 20;
        CrearTodasLasRanuras(cerdosRequeridos);
        
        playerReferencia = FindFirstObjectByType<Player>();
        if (playerReferencia != null)
        {
            ActualizarTextoMunicion(playerReferencia.currentAmmo, playerReferencia.stats.maxAmmo);
        }

        escalaOriginal = textoPuntuacion.transform.localScale;
    }
    private void Update()
    {
        if (playerReferencia == null)
        {
            playerReferencia = FindFirstObjectByType<Player>();
            if (playerReferencia == null) return;
        }

        if (textoMunicion != null)
        {
            int actual = playerReferencia.currentAmmo;
            int maximo = playerReferencia.stats.maxAmmo;
            textoMunicion.text = $"Ammo: {actual}/{maximo}";
        }
    }
    private void CrearTodasLasRanuras(int cantidad)
    {
        totalCerdos = cantidad;
        cerdosProcesados = 0;
        
        foreach (GameObject ranura in ranurasCerdos)
        {
            if (ranura != null)
                Destroy(ranura);
        }
        ranurasCerdos.Clear();
        for (int i = 0; i < totalCerdos; i++)
        {
            GameObject nuevoCerdoHUD = Instantiate(prefabCerdoNormal, contenedorCerdos);
            ranurasCerdos.Add(nuevoCerdoHUD);
        }
        
        StartCoroutine(AnimarAparicionRanuras());
    }
    
    private IEnumerator AnimarAparicionRanuras()
    {
        for (int i = 0; i < ranurasCerdos.Count; i++)
        {
            if (ranurasCerdos[i] != null)
            {
                Vector3 escalaOriginal = ranurasCerdos[i].transform.localScale;
                ranurasCerdos[i].transform.localScale = Vector3.zero;
                
                float tiempo = 0;
                while (tiempo < 0.1f)
                {
                    tiempo += Time.deltaTime;
                    float t = tiempo / 0.1f;
                    ranurasCerdos[i].transform.localScale = Vector3.Lerp(Vector3.zero, escalaOriginal, t);
                    yield return null;
                }
                ranurasCerdos[i].transform.localScale = escalaOriginal;
            }
            yield return new WaitForSeconds(0.03f);
        }
    }
    
    public void MarcarCerdoComoCazado(int numeroDeCerdo)
    {
        if (numeroDeCerdo >= 0 && numeroDeCerdo < ranurasCerdos.Count)
        {
            Transform ranuraActual = ranurasCerdos[numeroDeCerdo].transform;
            Vector3 posicion = ranuraActual.position;
            Quaternion rotacion = ranuraActual.rotation;
            Transform padre = ranuraActual.parent;
            
            Destroy(ranurasCerdos[numeroDeCerdo]);
            
            GameObject nuevaRanura = Instantiate(prefabCerdoCazado, posicion, rotacion, padre);
            
            ranurasCerdos[numeroDeCerdo] = nuevaRanura;
            
            StartCoroutine(EfectoDestello(nuevaRanura.GetComponent<Image>()));
        }
    }
    
    public void MarcarCerdoComoEscapado(int numeroDeCerdo)
    {
        if (numeroDeCerdo >= 0 && numeroDeCerdo < ranurasCerdos.Count)
        {
            Transform ranuraActual = ranurasCerdos[numeroDeCerdo].transform;
            Vector3 posicion = ranuraActual.position;
            Quaternion rotacion = ranuraActual.rotation;
            Transform padre = ranuraActual.parent;
            
            Destroy(ranurasCerdos[numeroDeCerdo]);
            
            GameObject nuevaRanura = Instantiate(prefabCerdoEscapado, posicion, rotacion, padre);
            ranurasCerdos[numeroDeCerdo] = nuevaRanura;
            
            StartCoroutine(EfectoDestello(nuevaRanura.GetComponent<Image>()));
        }
    }
    
    private IEnumerator EfectoDestello(Image imagen)
    {
        if (imagen == null) yield break;
        
        Color colorOriginal = imagen.color;
        imagen.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        imagen.color = colorOriginal;
    }
    
    public void ResetearRanuras()
    {
        CrearTodasLasRanuras(totalCerdos);
    }
    
    public void OnCerdoSpawned(int numeroDeCerdo)
    {
        if (numeroDeCerdo >= 0 && numeroDeCerdo < ranurasCerdos.Count)
        {
            StartCoroutine(ResaltarRanura(ranurasCerdos[numeroDeCerdo]));
        }
    }
    
    private IEnumerator ResaltarRanura(GameObject ranura)
    {
        if (ranura == null) yield break;
        
        Image img = ranura.GetComponent<Image>();
        if (img == null) yield break;
        
        Color colorOriginal = img.color;
        img.color = Color.yellow;
        
        Vector3 escalaOriginal = ranura.transform.localScale;
        ranura.transform.localScale = escalaOriginal * 1.3f;
        
        yield return new WaitForSeconds(0.15f);
        
        img.color = colorOriginal;
        ranura.transform.localScale = escalaOriginal;
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
        
        if (textoPuntuacion != null)
        {
            textoPuntuacion.color = colorNormalPuntuacion;
            textoPuntuacion.transform.localScale = Vector3.one;
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
        if (textoPuntuacion != null && !efectoEnProgreso) 
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
        {
            StopCoroutine(efectoPuntuacionActual);
            textoPuntuacion.color = colorNormalPuntuacion;
            textoPuntuacion.transform.localScale = Vector3.one;
            efectoEnProgreso = false;
        }
        
        efectoPuntuacionActual = StartCoroutine(EfectoSumarPuntos(puntosSumados));
    }
    
    private IEnumerator EfectoSumarPuntos(int puntos)
    {
        efectoEnProgreso = true;
        
        int puntajeActual = ScoreManager.Instance != null ? ScoreManager.Instance.GetPuntuacion() : 0;
        
        textoPuntuacion.color = colorAlSumarPuntos;
        textoPuntuacion.text = $"+{puntos}!";
        textoPuntuacion.transform.localScale = Vector3.one * 1.3f;
        
        yield return new WaitForSeconds(duracionEfectoColor);
        
        textoPuntuacion.color = colorNormalPuntuacion;
        textoPuntuacion.transform.localScale = Vector3.one;
        
        textoPuntuacion.text = $"Points: {puntajeActual}";
        
        efectoEnProgreso = false;
        efectoPuntuacionActual = null;
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