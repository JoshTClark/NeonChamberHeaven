using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyController : MonoBehaviour
{
    private CharacterController controller;
    private BaseEnemyBehavior behavior;

    [Header("Ground Checks")]
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float groundDistance;
    [SerializeField]
    private LayerMask groundMask;

    [Header("Movement Values")]
    [SerializeField]
    private float gravity;
    [SerializeField]
    private float maxFallSpeed;

    // Current velocity
    private Vector3 velocity;

    // The last recorded direction the enemy was moving
    private Vector3 movementResult;

    // Flags
    public bool isGrounded;

    private void Start()
    {
        controller = this.gameObject.GetComponent<CharacterController>();
        behavior = this.gameObject.GetComponent<BaseEnemyBehavior>();

        // Setting variables to default values
        velocity = new Vector3(0, 0, 0);
        isGrounded = false;
    }

    private void Update()
    {
        UpdateLookDirection();
        UpdateMovement();
    }

    /// <summary>
    /// Updates the enemies's looking direction
    /// </summary>
    private void UpdateLookDirection()
    {
        // Rotating the object
        Quaternion desiredRotation = Quaternion.LookRotation(behavior.LookDirection.normalized);

        //over time
        Quaternion currentRotation = Quaternion.Slerp(this.gameObject.transform.localRotation, desiredRotation, Time.deltaTime * behavior.LookSpeed);

        this.gameObject.transform.localRotation = currentRotation;
    }

    /// <summary>
    /// Updates the player's movement mechanics
    /// </summary>
    private void UpdateMovement()
    {
        // Updating flags
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Movement
        movementResult = behavior.Movement.normalized * behavior.Speed;

        // Gravity
        velocity.y += gravity * Time.deltaTime;

        // Grounded check
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2;
        }

        velocity.y = Mathf.Clamp(velocity.y, maxFallSpeed, float.MaxValue);

        controller.Move(movementResult * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);
    }
}
