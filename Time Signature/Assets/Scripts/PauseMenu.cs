using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public AudioSource menuOpenSound;
    private InputManager inputManager;


    // Start is called before the first frame update
    void Start()
    {
        pausePanel.SetActive(false);
        inputManager = new InputManager();
        inputManager.Enable();

    }

    // Update is called once per frame
    void Update()
    {
        if (pausePanel.activeInHierarchy)
        {
            Time.timeScale = 0;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel.activeSelf == false)
            {
                menuOpenSound.Play();
            }
            pausePanel.SetActive(true);
        }
    }
}
