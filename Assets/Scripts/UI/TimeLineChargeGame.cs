using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimelineCargadorJuego : MonoBehaviour
{
    public string nombreEscenaJuego = "Game";
    private PlayableDirector director;
    
    void Start()
    {
        director = GetComponent<PlayableDirector>();
        
        if (director != null)
        {
            director.stopped += OnTimelineTerminado;
        }
    }
    
    void OnTimelineTerminado(PlayableDirector pd)
    {
        SceneManager.LoadScene("Game");
    }
    
    void OnDestroy()
    {
        if (director != null)
        {
            director.stopped -= OnTimelineTerminado;
        }
    }
}