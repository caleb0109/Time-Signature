using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePosition : MonoBehaviour
{

    public Transform player;
    Vector3 playerPosition;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("playerStarted"))
        {
            player.position = new Vector3(PlayerPrefs.GetFloat("playerPositionX"), PlayerPrefs.GetFloat("playerPositionY"), PlayerPrefs.GetFloat("playerPositionZ"));
        }
        else
        {
            PlayerPrefs.SetInt("playerStarted", 1);
            PlayerPrefs.Save();
            player.position = new Vector3(-50.35f, -8.39f, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = player.position;
        PlayerPrefs.SetFloat("playerPositionX", playerPosition.x);
        PlayerPrefs.SetFloat("playerPositionY", playerPosition.y);
        PlayerPrefs.SetFloat("playerPositionZ", playerPosition.z);
        PlayerPrefs.Save();

    }
}
