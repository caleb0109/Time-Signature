using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerSpot;
    public Transform enemySpot;

    public BattleState state;

    public TMP_Text playerName;
    public TMP_Text enemyName;

    private Unit playerUnit;
    private Unit enemyUnit;

    private Animator enemyAnimator;
    private Animator playerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetUp());

        
    }


    IEnumerator SetUp()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerSpot);
        playerUnit = playerGO.GetComponent<Unit>();


        GameObject enemyGO = Instantiate(enemyPrefab, enemySpot);
        enemyUnit = enemyGO.GetComponent<Unit>();

        playerName.SetText(playerUnit.unitName);
        enemyName.SetText(enemyUnit.unitName);

        playerAnimator = playerGO.GetComponent<Animator>();
        enemyAnimator = enemyGO.GetComponent<Animator>();

        yield return new WaitForSeconds(1f);

        

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {

        playerAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(1f);
        //do the damage
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        Debug.Log(enemyUnit.currentHP);

        yield return new WaitForSeconds(2f);

        //Check if enemy is dead
        if(isDead)
        {
            //Change state to won if they are dead
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            //change state to enemy turn if they are not dead
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        
    }

    IEnumerator EnemyTurn()
    {
        //enemyPrefab
        Debug.Log("Enemy turn!");
        enemyAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(1f);

        //do the damage
        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
        Debug.Log(playerUnit.currentHP);

        yield return new WaitForSeconds(1f);

        if(isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void EndBattle()
    {
        //we can use this later for winning and losing battles
        Debug.Log("Battle over!");
    }
    

    void PlayerTurn()
    {
        Debug.Log("Player turn!");
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(PlayerAttack());
    }


}
