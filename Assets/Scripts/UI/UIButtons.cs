using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtons : MonoBehaviour
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

    public void reset()
    {
        SceneManager.LoadScene("Game");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
