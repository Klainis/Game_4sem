using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("Game1");

    }

    public void LoadGame()
    {
    }

    public void OpenSettings()
    {
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}