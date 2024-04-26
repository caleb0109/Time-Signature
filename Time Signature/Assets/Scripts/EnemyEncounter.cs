using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyEncounter : MonoBehaviour
{

    public static string enemyName;

    void Start()
    {

    }

    void OnCollisionEnter2D(Collision2D collision){
        
        enemyName = this.gameObject.ToString();
        if (PlayerPrefs.HasKey((enemyName + "Fought")))
        {
            Destroy(gameObject);
        }
        else{
            PlayerPrefs.SetInt((enemyName + "Fought"), 1);
            PlayerPrefs.Save();
            
            DontDestroyOnLoad(collision.gameObject);
            collision.gameObject.SetActive(false);
            Debug.Log(enemyName);
            SceneManager.LoadScene("Battle");
        }

           
            



    }
}
