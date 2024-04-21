using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public enum TutorialState { FREE, TALKING, ANIMATING}

public class Tutorial : MonoBehaviour
{

    private CharacterMovement rb;
    public TutorialState state;
    [SerializeField] private string conversationStartNode;
    
    
    [SerializeField] 
    private GameObject player;
    
    private DialogueRunner dialogueRunner;
    private bool isCurrentConversation;
    private bool interactable;
    private InputManager inputManager;


    // Start is called before the first frame update
    void Start()
    {
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        dialogueRunner.onDialogueComplete.AddListener(EndConversation);
        rb = player.GetComponent<CharacterMovement>();
        inputManager = new InputManager();
        inputManager.Enable();
        StartConversation();

    }

    

    // Update is called once per frame
    void Update()
    {

        if (state == TutorialState.TALKING || state == TutorialState.ANIMATING)
        {
            rb.enabled = false;
        }
        else
        {
            rb.enabled = true;
        }
    }

    private void StartConversation() {
        Debug.Log($"Started conversation with {name}.");
        rb.enabled = false;
        inputManager.Disable();
        //Debug.Log(inputManager.Character.Move);
        isCurrentConversation = true;
        state = TutorialState.TALKING;
        dialogueRunner.StartDialogue(conversationStartNode);
    }

    private void EndConversation() {
        if (isCurrentConversation) {
            isCurrentConversation = false;
            rb.enabled = true;
            Debug.Log($"Ended conversation with {name}.");
            EnableConversation();
        }
        
    }

    //[YarnCommand("enable")]
    public void EnableConversation() {
        interactable = true;
        state = TutorialState.FREE;
        inputManager.Enable();
    }

    //[YarnCommand("disable")]
    public void DisableConversation() {
        interactable = false;
    }
}
