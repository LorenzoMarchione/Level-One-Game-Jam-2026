using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("History");
    }
    public void Credits()
    {

    }
    public void Exit()
    {
        Application.Quit();
    }
}
