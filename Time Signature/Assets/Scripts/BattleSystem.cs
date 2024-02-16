using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public GameObject AttackButton;

    public Transform playerSpot;
    public Transform enemySpot;

    public BattleState state;

    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text enemyName;
    [SerializeField] TMP_Text dmgToEnemy;
    [SerializeField] TMP_Text dmgToPlayer;

    private Unit playerUnit;
    private Unit enemyUnit;

    private Animator enemyAnimator;
    private Animator playerAnimator;

    private RhythmManager rhythmMan;

    // Start is called before the first frame update
    void Start()
    {
        rhythmMan = this.GetComponent<RhythmManager>();
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

        dmgToEnemy.gameObject.SetActive(false);
        dmgToPlayer.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        rhythmMan.ShowExample();
        rhythmMan.BeginInput(null);

        //Waits for the rhythm game to be done before continuing
        while (!rhythmMan.isDone)
            yield return null;


        //formula for getting the damage done based on the rhythm
        float dmgTaken = Mathf.Round((playerUnit.damage * rhythmMan.score)/5);

        rhythmMan.isDone = false;
        playerAnimator.SetTrigger("Attack");
        yield return new WaitForSeconds(1f);
        dmgToEnemy.text = dmgTaken.ToString();
        dmgToEnemy.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        dmgToEnemy.gameObject.SetActive(false);
        //do the damage
        
        Debug.Log(dmgTaken);
        bool isDead = enemyUnit.TakeDamage((int)dmgTaken);
        Debug.Log(enemyUnit.currentHP);
        
        yield return new WaitForSeconds(1f);
        
        //yield return new WaitForSeconds(1f);
        //rhythmMan.score = 0;

        //Check if enemy is dead
        if(isDead)
        {
            //Change state to won if they are dead
            state = BattleState.WON;
            AttackButton.SetActive(false);
            EndBattle();
        }
        else
        {
            //change state to enemy turn if they are not dead
            state = BattleState.ENEMYTURN;
            AttackButton.SetActive(false);
            StartCoroutine(EnemyTurn());
        }
        
    }

    IEnumerator EnemyTurn()
    {
        //enemyPrefab
        Debug.Log("Enemy turn!");
        enemyAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(1f);

        dmgToPlayer.text = enemyUnit.damage.ToString();
        dmgToPlayer.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        dmgToPlayer.gameObject.SetActive(false);

        //do the damage
        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
        Debug.Log(playerUnit.currentHP);

        yield return new WaitForSeconds(1f);

        if(isDead)
        {
            state = BattleState.LOST;
            AttackButton.SetActive(false);
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
        AttackButton.SetActive(true);
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN){
            return;
        }
        AttackButton.SetActive(false);
        StartCoroutine(PlayerAttack());
    }


}
