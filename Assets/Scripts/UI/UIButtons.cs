using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtons : MonoBehaviour
{
    public GameObject pal;

    public void PlayGame()
    {
        SceneManager.LoadScene("History");
    }
    public void Credits()
    {
        pal.SetActive(true);
    }

    public void Close()
    {
        pal.SetActive(false);
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

    public void Level2()
    {
        SceneManager.LoadScene("Game 1");
    }
}
