using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;
    public Vector2 inputDirection;

    [Header("Basic parameters")]
    public float speed;
    public float jumpForce;
    public Rigidbody2D rb;
    private SpriteRenderer rbSprite;

    private void Awake()
    {
        inputControl = new PlayerInputControl();
        inputControl.Gameplay.Jump.started += Jump;

        rbSprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    private void Update()
    {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime,rb.velocity.y);

        //�����泯����

        if (inputDirection.x > 0)
            rbSprite.flipX = false;
        if(inputDirection.x < 0)
            rbSprite.flipX = true;
        //���﷭ת
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        //Debug.Log("JUMP");
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }
}
