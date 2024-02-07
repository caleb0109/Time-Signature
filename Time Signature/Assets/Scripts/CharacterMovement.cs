using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{

    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float viewAngle;
    private float verticalWalkSpeed;

    private InputManager inputManager;
    private Vector2 targetVelocity;

    private Vector2 velocity;
    private Vector2 cameraVelocity;

    [SerializeField]
    private float accelerationSpeed;
    [SerializeField]
    private float cameraAcceleration;

    [SerializeField]
    private GameObject cam;
    // Start is called before the first frame update
    void Start()
    {
        //setup input handlers
        inputManager = new InputManager();
        inputManager.Enable();
        inputManager.Character.Move.performed += ctx => Move(ctx);

        //determine how much to move vertically to represent depth
        verticalWalkSpeed = Mathf.Sin(viewAngle);
    }

    // Update is called once per frame
    void Update()
    {
        // Update the character's velocity to approach the target
        velocity = Vector2.Lerp(velocity, targetVelocity, Time.deltaTime * accelerationSpeed);
        // Camera velocity is updated by a fraction of the character's acceleration speed so it lags behind
        cameraVelocity = Vector2.Lerp(cameraVelocity, targetVelocity, Time.deltaTime * cameraAcceleration * accelerationSpeed);
        // Camera doesn't move up or down
        cameraVelocity.y = 0;

        // Move the character and camera
        transform.Translate(velocity * Time.deltaTime);
        cam.transform.Translate(cameraVelocity * Time.deltaTime);
    }

    void Move(InputAction.CallbackContext inputCtx)
    {
        //update the speed the player wants to move at
        Vector2 input = inputCtx.ReadValue<Vector2>();
        targetVelocity = new Vector2(input.x, input.y * verticalWalkSpeed) * walkSpeed;
    }
}
