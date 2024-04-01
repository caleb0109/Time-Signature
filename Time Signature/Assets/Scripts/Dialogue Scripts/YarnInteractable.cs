using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.InputSystem;

public class YarnInteractable : MonoBehaviour {
    // internal properties exposed to editor
    [SerializeField] private string conversationStartNode;
    [SerializeField] private bool beginsInteractable;
    [SerializeField] private bool useIndicatorLight;
    [SerializeField] private GameObject player;

    // internal properties not exposed to editor
    private DialogueRunner dialogueRunner;
    private Light lightIndicatorObject;
    private InputManager inputManager;
    private bool interactable;
    private bool isCurrentConversation;
    private float defaultIndicatorIntensity;

    public void Start() {
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        dialogueRunner.onDialogueComplete.AddListener(EndConversation);
        //lightIndicatorObject = GetComponentInChildren<Light>();
        interactable = beginsInteractable;

        inputManager = new InputManager();
        inputManager.Enable();
        inputManager.Character.Interact.performed += ctx => Interact(ctx);
    }

    public void Interact(InputAction.CallbackContext inputCtx) {
        if (interactable && !dialogueRunner.IsDialogueRunning && this.transform.position.x - player.transform.position.x > -2 && this.transform.position.x - player.transform.position.x < 2 && this.transform.position.y - player.transform.position.y > -1 && this.transform.position.y - player.transform.position.y < 1) {
            StartConversation();
            inputManager.Disable();
        }
    }

    private void StartConversation() {
        Debug.Log($"Started conversation with {name}.");
        
        Debug.Log(inputManager.Character.Move);
        isCurrentConversation = true;
        dialogueRunner.StartDialogue(conversationStartNode);
    }

    private void EndConversation() {
        if (isCurrentConversation) {
            isCurrentConversation = false;
            Debug.Log($"Ended conversation with {name}.");
            
            
        }
        
    }

    [YarnCommand("enable")]
    public void EnableConversation() {
        interactable = true;
        inputManager.Enable();
    }

    [YarnCommand("disable")]
    public void DisableConversation() {
        interactable = false;
    }
}
