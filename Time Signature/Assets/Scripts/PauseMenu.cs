using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public AudioSource menuOpenSound;
    public GameObject soundContainer;

    private bool disablePause;
    private bool gamePaused;

    private AudioSource music;
    private AudioSource secondMusic;
    private InputManager inputManager;

    // Start is called before the first frame update
    void Start()
    {
        secondMusic = null;

        pausePanel.SetActive(false);

        inputManager = new InputManager();
        inputManager.Enable();

        music = soundContainer.transform.Find("Music Audio").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!music.isActiveAndEnabled)
        {
            music = soundContainer.transform.GetChild(6).GetComponent<AudioSource>();
            secondMusic = soundContainer.transform.GetChild(7).GetComponent<AudioSource>();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !disablePause)
        {
            if (pausePanel.activeSelf == false)
            {
                menuOpenSound.Play();
                music.Pause();

                if (secondMusic != null)
                {
                    secondMusic.Pause();
                }

                pausePanel.SetActive(true);

                gamePaused = true;
                Time.timeScale = 0;
            }
            else if (pausePanel.activeSelf == true)
            {
                Time.timeScale = 1;
                gamePaused = false;
                music.UnPause();

                if (secondMusic != null)
                {
                    secondMusic.Pause();
                }

                pausePanel.transform.Find("Settings Background").gameObject.SetActive(false);
                pausePanel.SetActive(false);
            }
        }
    }

    //Detects if the player is attacking and disables the pause function if so. 
    public bool DisablePause
    {
        get
        {
            return disablePause;
        }
        set
        {
            disablePause = value;
        }
    }

    //Get-Set function corresponding to the game's paused status.
    public bool GamePaused
    {
        get
        {
            return gamePaused;
        }
        set
        {
            gamePaused = value;
        }
    }
}
