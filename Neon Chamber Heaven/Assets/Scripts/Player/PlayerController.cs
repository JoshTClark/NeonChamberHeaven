using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Header("Player Stats Holder")]
    private PlayerStats stats;

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

    [Header("Bullet Prefab")]
    [SerializeField]
    private PlayerBulletController bulletPrefab;

    [Header("Bullet Visual Starting Point")]
    [SerializeField]
    private GameObject bulletStart;

    [Header("Movement Values")]
    [SerializeField]
    private float gravity;
    [SerializeField]
    private float slowFallSpeed;
    [SerializeField]
    private float maxFallSpeed;
    [SerializeField]
    private float dashSpeed;
    [SerializeField]
    private float dashActiveTime;

    // Controls input
    private GameControls input;

    // Current direction to look at
    private Vector2 lookDirection;

    // Current velocity
    private Vector3 velocity;

    // The last recorded direction the player was moving
    private Vector2 movementInput;
    private Vector3 movementResult;

    // Timers
    private float reloadTimer, fireRateTimer, dashActiveTimer;

    // Flags
    public bool isGrounded;
    public bool jumpHeld;
    public bool isSlamming;
    public bool isDashing;
    public bool isReloading;
    public bool isFireRateCooldown;

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

    private void Update()
    {
        UpdateLookDirection();
        UpdateMovement();
        UpdateGun();
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

        // Normal movement - has no acceleration
        if (!isDashing)
        {
            movementInput = input.CharacterControls.Move.ReadValue<Vector2>() * stats.Speed;
            movementResult = movementInput.x * transform.right + movementInput.y * transform.forward;
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;

        // If jump is held clamp falling speed
        if (((jumpHeld && !isSlamming) || (input.CharacterControls.Jump.WasPressedThisFrame() && isSlamming)) && !isDashing && !isGrounded)
        {
            velocity.y = Mathf.Clamp(velocity.y, slowFallSpeed, float.MaxValue);
            isSlamming = false;
            if (velocity.y < 0)
            {
                stats.Stamina -= (stats.FloatStaminaCost * Time.deltaTime);
            }
        }

        // Jump
        if (input.CharacterControls.Jump.WasPressedThisFrame() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(stats.JumpHeight * -2f * gravity);
        }

        // Slam
        if (input.CharacterControls.Slam.WasPressedThisFrame() && !isGrounded && !isSlamming && stats.Stamina >= stats.SlamStaminaCost)
        {
            stats.Stamina -= stats.SlamStaminaCost;
            velocity.y = maxFallSpeed;
            movementInput *= 0.5f;
            isSlamming = true;
            if (isDashing)
            {
                isDashing = false;
                dashActiveTimer = 0.0f;
            }
        }

        // Grounded check
        if (isGrounded && velocity.y < 0)
        {
            isSlamming = false;
            velocity.y = -2;
        }

        // Dashing movement
        if (input.CharacterControls.Dash.WasPressedThisFrame() && !isDashing && stats.Stamina >= stats.DashStaminaCost)
        {
            stats.Stamina -= stats.DashStaminaCost;
            isDashing = true;
            movementInput = input.CharacterControls.Move.ReadValue<Vector2>() * dashSpeed;
            movementResult = movementInput.x * transform.right + movementInput.y * transform.forward;
        }
        else if (isDashing)
        {
            velocity.y = 0f;
            dashActiveTimer += Time.deltaTime;
            if (dashActiveTimer >= dashActiveTime)
            {
                isDashing = false;
                dashActiveTimer = 0.0f;
            }
        }

        velocity.y = Mathf.Clamp(velocity.y, maxFallSpeed, float.MaxValue);

        controller.Move(movementResult * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Updates shooting, reload time, fire rate
    /// </summary>
    private void UpdateGun()
    {
        // Reloading
        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0)
            {
                isReloading = false;
                stats.CurrentAmmo = stats.MaxAmmo;
            }
        }

        // Fire rate
        if (isFireRateCooldown)
        {
            fireRateTimer -= Time.deltaTime;
            if (fireRateTimer <= 0)
            {
                isFireRateCooldown = false;
            }
        }

        // If is neither reloading or on cooldown the player can shoot
        if (!isReloading && !isFireRateCooldown)
        {
            // The player pressed shoot
            if (input.CharacterControls.Shoot.WasPressedThisFrame())
            {
                this.gameObject.GetComponent<Animator>().SetTrigger("Shoot");
                this.gameObject.GetComponent<Animator>().SetFloat("GunShootSpeed", 1f / stats.FireRate);

                isFireRateCooldown = true;
                fireRateTimer = stats.FireRate;
                stats.CurrentAmmo--;

                if (stats.CurrentAmmo <= 0)
                {
                    reloadTimer = stats.ReloadSpeed;
                    isReloading = true;
                }

                PlayerBulletController bullet = GameObject.Instantiate(bulletPrefab);
                bullet.DoBulletShot(playerCamera.transform.position, playerCamera.transform.forward, 2, bulletStart.transform.position);
            }
        }
    }

    public float GetPercentDoneReloading()
    {
        return reloadTimer / stats.ReloadSpeed;
    }

    public float GetPercentStaminaRemaining()
    {
        return stats.Stamina / stats.MaxStamina;
    }
}
