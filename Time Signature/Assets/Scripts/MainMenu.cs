using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //Helper variables that will allow us to find and disable/enable panels.
    private GameObject displayPanel;
    private GameObject controlsPanel;
    private GameObject audioPanel;

    private void Start()
    {
        //Finds the three settings panels and initializes their correponding variables.
        displayPanel = GameObject.Find("Display Panel");
        controlsPanel = GameObject.Find("Controls Panel");
        audioPanel = GameObject.Find("Audio Panel");
        
        //Hides the panels before leaving the display panel active. This will ensure it stays open.
        HidePanels();
        displayPanel.SetActive(true);

        //Disables the settings background, leaving only the main meny on screen.
        GameObject.Find("Settings Background").SetActive(false);
        if (GameObject.Find("Credits Menu"))
        {
            GameObject.Find("Credits Menu").SetActive(false);
        }
        
    }

    //A helper function that hides each panel on the screen.
    private void HidePanels()
    {
        displayPanel.SetActive(false);
        controlsPanel.SetActive(false);
        audioPanel.SetActive(false);
    }

    //A function that, when triggered, will change the scene.
    public void GoToScene(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    //A function that exists the game.
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("You quit the game!");
    }

    //A function that toggles if the settings menu is displayed or not.
    public void ToggleSettings(GameObject objectName)
    {
        objectName.SetActive(!objectName.activeInHierarchy);
    }

    /*A function that, when activated, switches the settings tab to the one matching
    the button pressed*/
    public void SwitchTab(GameObject enabledPanel)
    {
        if (!enabledPanel.activeInHierarchy)
        {
            HidePanels();

            enabledPanel.SetActive(true);
        }
    }

    public void UnPause(GameObject objectName)
    {
        Time.timeScale = 1;
        
        Debug.Log(Time.timeScale.ToString());
        objectName.SetActive(false);
    }
}
