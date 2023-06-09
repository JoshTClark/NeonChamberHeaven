using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class BaseEnemyBehavior : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField]
    private float speed;
    [SerializeField]
    private float lookSpeed;

    private PlayerController player;
    private Vector3 movement = new Vector3();
    private Vector3 lookDirection = new Vector3();

    public Vector3 Movement
    {
        get { return movement; }
    }

    public Vector3 LookDirection
    {
        get { return lookDirection; }
    }

    public float Speed
    {
        get { return speed; }
    }

    public float LookSpeed
    {
        get { return lookSpeed; }
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        UpdateLookDirection();
        UpdateMovement();
    }

    private void UpdateLookDirection() 
    {
        lookDirection = player.transform.position - this.transform.position;
        lookDirection.y = 0f;
    }

    private void UpdateMovement()
    {
        movement = lookDirection;
    }
}
