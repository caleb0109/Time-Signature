using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public AudioSource menuOpenSound;
    public GameObject soundContainer;

    private bool playerAttacking;

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
        //Debug.Log(playerAttacking + " THING");

        if (!music.isActiveAndEnabled)
        {
            music = soundContainer.transform.GetChild(6).GetComponent<AudioSource>();
            secondMusic = soundContainer.transform.GetChild(7).GetComponent<AudioSource>();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !playerAttacking)
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

                Time.timeScale = 0;
            }
            else if (pausePanel.activeSelf == true)
            {
                Time.timeScale = 1;
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
    public bool PlayerAttacking
    {
        get
        {
            return playerAttacking;
        }
        set
        {
            playerAttacking = value;
        }
    }
}
