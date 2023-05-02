using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Header("Control Options")]
    private Vector2 lookSensitvity;

    [SerializeField, Header("Character Controller")]
    private CharacterController controller;

    [SerializeField, Header("Camera")]
    private Camera playerCamera;

    [Header("Ground and Wall Checks")]
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float groundDistance;
    [SerializeField]
    private LayerMask groundMask;


    [Header("Movement Values")]
    [SerializeField]
    private float speed;
    [SerializeField]
    private float gravity;
    [SerializeField]
    private float jumpHeight;
    [SerializeField]
    private float slowFallSpeed;

    // Controls input
    private GameControls input;

    // Current direction to look at
    private Vector2 lookDirection;

    // Current velocity
    private Vector3 velocity;

    // Flags
    private bool isGrounded;
    private bool jumpHeld;

    private void Start()
    {
        // Locks the cursor to the game screen
        Cursor.lockState = CursorLockMode.Locked;

        // Setting up and enabling the controls
        input = new GameControls();
        input.CharacterControls.Enable();

        // Setting variables to default values
        lookDirection = new Vector2(0, 0);
        velocity = new Vector3(0, 0, 0);
        isGrounded = false;
    }

    public void Update()
    {
        UpdateLookDirection();
        UpdateMovement();
    }

    /// <summary>
    /// Updates the player's looking direction
    /// Rotates the player object and camera accordingly
    /// </summary>
    private void UpdateLookDirection()
    {
        // Getting the look direction delta and updating the look direction
        Vector2 lookChange = input.CharacterControls.Look.ReadValue<Vector2>() * lookSensitvity * Time.deltaTime;
        lookDirection.x += lookChange.x;
        lookDirection.y -= lookChange.y;
        lookDirection.y = Mathf.Clamp(lookDirection.y, -90f, 90f);

        // Player object should only be rotated along the Y axis
        this.gameObject.transform.localRotation = Quaternion.Euler(0f, lookDirection.x, 0f);
        // Camera should only be rotated along the X axis
        playerCamera.gameObject.transform.localRotation = Quaternion.Euler(lookDirection.y, 0f, 0f);
    }

    /// <summary>
    /// Updates the player's movement mechanics
    /// </summary>
    private void UpdateMovement()
    {
        // Updating flags
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        jumpHeld = input.CharacterControls.Jump.ReadValue<float>() != 0f;

        Vector2 movementInput = input.CharacterControls.Move.ReadValue<Vector2>() * speed * Time.deltaTime;
        Vector3 movementResult = movementInput.x * transform.right + movementInput.y * transform.forward;

        velocity.y += gravity * Time.deltaTime;

        // If jump is held clamp falling speed
        if (jumpHeld)
        {
            velocity.y = Mathf.Clamp(velocity.y, slowFallSpeed, float.MaxValue);
        }

        if (input.CharacterControls.Jump.WasPressedThisFrame() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2;
        }

        controller.Move(movementResult);
        controller.Move(velocity * Time.deltaTime);
    }
}
