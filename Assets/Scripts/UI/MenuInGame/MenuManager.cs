using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private EventSystem eventSystem;

    private RTSCameraController rTSCameraController;

     void Start()
    {
        rTSCameraController = FindObjectOfType<RTSCameraController>(); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && menuPanel.activeSelf)
        {
            menuPanel.SetActive(false);
            CloseMenu();
        }
    }

    public void OpenMenu()
    {
        eventSystem.SetSelectedGameObject(menuPanel);
        if (rTSCameraController != null)
            rTSCameraController.enabled = false;
    }

    public void CloseMenu()
    {
        eventSystem.SetSelectedGameObject(null);
        if (rTSCameraController != null)
            rTSCameraController.enabled = true;
    }

    public void NewGame()   
    {
        SceneManager.LoadScene("Game1");

    }

    public void QuitGame()
    {
        //Application.Quit();
        SceneManager.LoadScene("Main Menu");
    }
}
