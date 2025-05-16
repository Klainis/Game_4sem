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
        // Реализация загрузки (используйте PlayerPrefs или систему сохранений)
    }

    public void OpenSettings()
    {
        // Активируйте Panel с настройками (см. ниже)
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}