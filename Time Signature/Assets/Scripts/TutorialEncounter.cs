using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialEncounter : MonoBehaviour
{

    public static string enemyName;

    void Start()
    {
        enemyName = this.gameObject.ToString();
        if (PlayerPrefs.HasKey((enemyName + "Tutorial")))
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision){
        enemyName = this.gameObject.ToString();


        PlayerPrefs.SetInt((enemyName + "Tutorial"), 1);
        PlayerPrefs.Save();
            
        DontDestroyOnLoad(collision.gameObject);
        collision.gameObject.SetActive(false);
        Debug.Log(enemyName);
        SceneManager.LoadScene("TutorialBattle");
        
    }
}
