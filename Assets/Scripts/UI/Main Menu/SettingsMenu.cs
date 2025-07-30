using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject menuPanel;
    public GameObject textNameOfGame;

    public void ToggleSettings()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        menuPanel.SetActive(!menuPanel.activeSelf);
        textNameOfGame.SetActive(!textNameOfGame.activeSelf);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}