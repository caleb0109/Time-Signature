using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyEncounter : MonoBehaviour
{

    public static string enemyName;

    void OnCollisionEnter2D(Collision2D collision){
        enemyName = this.gameObject.ToString();
        Debug.Log(enemyName);
        SceneManager.LoadScene("Battle");
    }
}
