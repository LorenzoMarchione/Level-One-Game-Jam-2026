using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ImageTransition : MonoBehaviour
{
    [Header("Configuración")]
    public PlayableDirector timeline;
    public float duracionTimeline = 5f;
    
    [Header("Escenas")]
    public string nombreEscenaJuego = "Game";
    
    [Header("Opcional - Para imágenes específicas")]
    public RawImage image1;
    public RawImage image2;
    
    private void Start()
    {
        if (image2 != null)
        {
            Color color = image2.color;
            color.a = 0f;
            image2.color = color;
        }
        
        if (image1 != null)
        {
            Color color = image1.color;
            color.a = 1f;
            image1.color = color;
        }
        
        if (timeline != null)
        {
            timeline.Play();
            Debug.Log($"Timeline iniciada, durará: {duracionTimeline} segundos");
            
            StartCoroutine(CargarJuegoDespuesDeTimeline());
        }
        else
        {
            Debug.LogError("No se asignó la Timeline en el Inspector!");
        }
    }
    
    private IEnumerator CargarJuegoDespuesDeTimeline()
    {
        yield return new WaitForSeconds(duracionTimeline);
        
        Debug.Log($"Cargando escena: {nombreEscenaJuego}");
        SceneManager.LoadScene(nombreEscenaJuego);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaltarTimeline();
        }
    }
    
    public void SaltarTimeline()
    {
        Debug.Log("Saltando Timeline...");
        StopAllCoroutines();
        
        if (timeline != null && timeline.state == PlayState.Playing)
        {
            timeline.Stop();
        }
        
        SceneManager.LoadScene(nombreEscenaJuego);
    }
    
    public void PausarTimeline()
    {
        if (timeline != null)
        {
            timeline.Pause();
        }
    }
    
    public void ReanudarTimeline()
    {
        if (timeline != null)
        {
            timeline.Resume();
        }
    }
}