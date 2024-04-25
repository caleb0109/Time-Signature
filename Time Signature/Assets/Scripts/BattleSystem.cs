using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject Wolf;
    public GameObject Slime;
    private GameObject enemyPrefab;

    public GameObject AttackButton;
    public GameObject MagicButton;

    public GameObject attackDisplay;
    public GameObject feedbackText;

    public GameObject healthBar;

    public Transform playerSpot;
    public Transform enemySpot;

    public BattleState state;

    [SerializeField] TMP_Text dmgToEnemy;
    [SerializeField] TMP_Text dmgToPlayer;
    [SerializeField] TMP_Text currentMaxHp;

    private Unit playerUnit;
    private Unit enemyUnit;

    private Animator enemyAnimator;
    private Animator playerAnimator;

    private RhythmManager rhythmMan;

    [SerializeField] private GameObject beatButton;
    [SerializeField] private GameObject beatSelector;
    [SerializeField] private GameObject magicBeatSelector;
    private RectTransform beatSelectorContent;
    private RectTransform magicSelectorContent;

    private BeatManager beatManager;

    // Start is called before the first frame update
    void Start()
    {
        beatManager = FindAnyObjectByType<BeatManager>();
        rhythmMan = this.GetComponent<RhythmManager>();
        state = BattleState.START;
        beatSelectorContent = beatSelector.GetComponent<ScrollRect>().content;
        magicSelectorContent = magicBeatSelector.GetComponent<ScrollRect>().content;
        StartCoroutine(SetUp());
    }

    IEnumerator SetUp()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerSpot);
        playerUnit = playerGO.GetComponent<Unit>();
        Debug.Log(EnemyEncounter.enemyName);
        if (EnemyEncounter.enemyName == "Wolf (UnityEngine.GameObject)")
        {
            enemyPrefab = Wolf;

            enemyPrefab.transform.localScale = new Vector3(5, 5, 5);
        }
        else
        {
            enemyPrefab = Slime;

            enemyPrefab.transform.localScale = new Vector3(2, 2, 2);
        }
        GameObject enemyGO = Instantiate(enemyPrefab, enemySpot);
        enemyUnit = enemyGO.GetComponent<Unit>();

        playerAnimator = playerGO.GetComponent<Animator>();
        enemyAnimator = enemyGO.GetComponent<Animator>();

        dmgToEnemy.gameObject.SetActive(false);
        dmgToPlayer.gameObject.SetActive(false);

        UpdateHealthBar(playerUnit.currentHP);

        GenerateBeatSelector();

        beatSelector.SetActive(false);
        magicBeatSelector.SetActive(false);

        yield return new WaitForSeconds(0.25f);



        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void GenerateBeatSelector()
    {
        string[] attacks = beatManager.GetAllAttackNames();
        int attackCount = attacks.Length;
        float viewportHeight = -beatSelectorContent.offsetMin.y;

        RectTransform buttonPrefabTransform = beatButton.GetComponent<RectTransform>();
        float buttonHeight = buttonPrefabTransform.sizeDelta.y;
        float nextAttackButtonPos = viewportHeight/2.0f - buttonHeight/2.0f - 5.0f;
        float nextMagicButtonPos = viewportHeight/2.0f - buttonHeight/2.0f - 5.0f;
        for(int i = 0; i < attackCount; i++)
        {
            bool magic = beatManager.GetAttack(i).magic;
            GameObject buttonObj;
            if(magic)
            {
                buttonObj = Instantiate(beatButton, magicSelectorContent);
            }
            else
            {
                buttonObj = Instantiate(beatButton, beatSelectorContent);
            }
            Button buttonButton = buttonObj.GetComponent<Button>();
            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();

            int param = i;
            buttonButton.onClick.AddListener(() => {PickAttack(param);});
            buttonText.text = beatManager.GetAttack(i).name;
            if(magic)
            {
                buttonRect.anchoredPosition = new Vector2(0, nextMagicButtonPos);
                nextMagicButtonPos -= buttonHeight + 5.0f;
            }
            else
            {
                buttonRect.anchoredPosition = new Vector2(0, nextAttackButtonPos);
                nextAttackButtonPos -= buttonHeight + 5.0f;
            }
        }
    }

    private void PlayerAttack()
    {
        beatSelector.SetActive(true);
        magicBeatSelector.SetActive(false);
    }

    private void PlayerMagicAttack()
    {
        magicBeatSelector.SetActive(true);
        beatSelector.SetActive(false);
    }

    public void PlayerRhythmFinished(float score)
    {
        StartCoroutine(AnimatePlayerAttack(score));
    }

    private IEnumerator AnimatePlayerAttack(float score)
    {
        //formula for getting the damage done based on the rhythm
        float dmgTaken = Mathf.Round((playerUnit.damage * score)/5);

        playerAnimator.SetTrigger("Attack");
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<PauseMenu>().PlayerAttacking = false;
        feedbackText.GetComponent<TextMeshProUGUI>().text = "";
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
            enemyUnit.gameObject.SetActive(false);
            AttackButton.GetComponent<Button>().interactable = false;
            EndBattle();
        }
        else
        {
            //change state to enemy turn if they are not dead
            state = BattleState.ENEMYTURN;
            AttackButton.GetComponent<Button>().interactable = false;
            StartCoroutine(EnemyTurn());
        }
    }

    public void PickAttack(int beatIndex)
    {
        beatSelector.SetActive(false);
        magicBeatSelector.SetActive(false);
        AttackButton.GetComponent<Button>().interactable = false;
        MagicButton.GetComponent<Button>().interactable = false;
        rhythmMan.SetBeat(beatManager.GetAttack(beatIndex));
        gameObject.GetComponent<PauseMenu>().PlayerAttacking = true;
        rhythmMan.BeginBeat(PlayerRhythmFinished);
    }

    IEnumerator EnemyTurn()
    {
        //enemyPrefab
        Debug.Log("Enemy turn!");
        enemyAnimator.SetTrigger("Attack");
        playerAnimator.SetTrigger("Damage");

        yield return new WaitForSeconds(1f);

        dmgToPlayer.text = enemyUnit.damage.ToString();
        dmgToPlayer.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        dmgToPlayer.gameObject.SetActive(false);

        //do the damage
        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
        UpdateHealthBar(playerUnit.currentHP);
        Debug.Log(playerUnit.currentHP);

        yield return new WaitForSeconds(1f);

        if(isDead)
        {
            state = BattleState.LOST;
            playerUnit.gameObject.SetActive(false);
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
        SceneManager.LoadScene("TestLevel");
    }


    void PlayerTurn()
    {

        Debug.Log("Player turn!");
        AttackButton.GetComponent<Button>().interactable = true;
        MagicButton.GetComponent<Button>().interactable = true;
        attackDisplay.SetActive(false);
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }

        attackDisplay.SetActive(true);
        attackDisplay.transform.GetChild(1).gameObject.SetActive(false);

        AttackButton.GetComponent<Button>().interactable = false;
        MagicButton.GetComponent<Button>().interactable = false;
        PlayerAttack();
    }

    public void OnMagicButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }

        attackDisplay.SetActive(true);
        attackDisplay.transform.GetChild(1).gameObject.SetActive(true);
        MagicButton.GetComponent<Button>().interactable = false;
        AttackButton.GetComponent<Button>().interactable = false;
        PlayerMagicAttack();
    }

    public void OnBackButtton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }

        attackDisplay.SetActive(false);
        MagicButton.GetComponent<Button>().interactable = true;
        AttackButton.GetComponent<Button>().interactable = true;

        beatSelector.SetActive(false);
        magicBeatSelector.SetActive(false);
    }

    //A function that updates the UI displaying the player's HP.
    private void UpdateHealthBar(int currentHP)
    {
        float healthRatio;

        //If the player has less then 0 health, their HP will be considered as 0.
        if (currentHP <= 0)
        {
            currentHP = 0;
        }

        /*Finds the ratio between the player's current HP and max HP before determining the offset
        for their health bar.*/
        healthRatio = currentHP / float.Parse(playerUnit.maxHP.ToString());
        float widthChange = 107 - 107 * healthRatio;

        //Updates the text display and applies the offset to the health bar.
        currentMaxHp.text = currentHP + "/" + playerUnit.maxHP;
        healthBar.GetComponent<RectTransform>().offsetMax = new Vector2(widthChange * -1, healthBar.GetComponent<RectTransform>().offsetMax.y);
    }
}
