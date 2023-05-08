using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField]
    private float maxHP;
    [SerializeField]
    private float maxStamina;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpHeight;
    [SerializeField]
    private float reloadSpeed;
    [SerializeField]
    private float fireRate;

    [Header("Stamina Usage")]
    [SerializeField]
    private float staminaRegen;
    [SerializeField]
    private float dashStaminaCost;
    [SerializeField]
    private float floatStaminaCost;
    [SerializeField]
    private float slamStaminaCost;

    // HP + Stamina counters
    private float hp, stamina;

    // Ammo counters
    private int maxAmmo = 6, currentAmmo = 6;

    // Properties
    public float Speed
    {
        get { return speed; }
    }

    public float JumpHeight
    {
        get { return jumpHeight; }
    }

    public float ReloadSpeed
    {
        get { return reloadSpeed; }
    }

    public float FireRate
    {
        get { return fireRate;  }
    }

    public float BaseFireRate
    {
        get { return fireRate; }
    }

    public int MaxAmmo
    {
        get { return maxAmmo; }
    }

    public int CurrentAmmo
    {
        get { return currentAmmo; }
        set { currentAmmo = value; }
    }

    public float MaxHP
    {
        get { return maxHP; }
    }

    public float HP
    {
        get { return hp; }
        set { hp = value; }
    }


    public float MaxStamina
    {
        get { return maxStamina; }
    }

    public float Stamina
    {
        get { return stamina; }
        set { stamina = value; }
    }

    public float StaminaRegen
    {
        get { return staminaRegen; }
    }

    public float DashStaminaCost
    {
        get { return dashStaminaCost; }
    }

    public float FloatStaminaCost
    {
        get { return floatStaminaCost; }
    }

    public float SlamStaminaCost
    {
        get { return slamStaminaCost; }
    }


    /// <summary>
    /// Sets stuff like hp and stamina
    /// </summary>
    private void Start()
    {
        hp = maxHP;
        stamina = maxStamina;
    }

    /// <summary>
    /// Basic update logic
    /// Logic should only go here if it directly modifies a stat counter
    /// </summary>
    private void Update()
    {
        stamina += (StaminaRegen * Time.deltaTime);
        stamina = Mathf.Clamp(stamina, 0, MaxStamina);
    }
}
