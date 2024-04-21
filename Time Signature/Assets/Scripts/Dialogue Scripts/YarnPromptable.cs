using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.InputSystem;

public class YarnPromptable : MonoBehaviour {
    // internal properties exposed to editor
    [SerializeField] private string conversationStartNode;
    [SerializeField] private bool beginsInteractable;

    [SerializeField] CharacterMovement movement;

    // internal properties not exposed to editor
    private DialogueRunner dialogueRunner;
    private bool isCurrentConversation;

    [SerializeField] 
    private Tutorial tutorial;

    public void Start() {
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        dialogueRunner.onDialogueComplete.AddListener(EndConversation);

    }





    void OnTriggerEnter2D(Collider2D collision){
        StartConversation();
    }



    private void StartConversation() {
        Debug.Log($"Started conversation with {name}.");
        isCurrentConversation = true;
        dialogueRunner.StartDialogue(conversationStartNode);
        tutorial.state = TutorialState.TALKING;
    }

    private void EndConversation() {
        if (isCurrentConversation) {
            isCurrentConversation = false;
            Debug.Log($"Ended conversation with {name}.");
            EnableConversation();
        }
        
    }

    //[YarnCommand("enable")]
    public void EnableConversation() {
        
        tutorial.state = TutorialState.FREE;
    }

    //[YarnCommand("disable")]
    public void DisableConversation() {
        
    }
}
