using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPosition : MonoBehaviour
{

    public Transform player;
    Vector3 playerPosition;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("tutorialStarted"))
        {
            player.position = new Vector3(PlayerPrefs.GetFloat("tutorialPositionX"), PlayerPrefs.GetFloat("tutorialPositionY"), PlayerPrefs.GetFloat("tutorialPositionZ"));
        }
        else
        {
            PlayerPrefs.SetInt("tutorialStarted", 1);
            PlayerPrefs.Save();
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = player.position;
        PlayerPrefs.SetFloat("tutorialPositionX", playerPosition.x);
        PlayerPrefs.SetFloat("tutorialPositionY", playerPosition.y);
        PlayerPrefs.SetFloat("tutorialPositionZ", playerPosition.z);
        PlayerPrefs.Save();

    }
}
