using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;
    public Vector2 inputDirection;
    private PhysicsCheck physicsCheck;
    public Rigidbody2D rb;
    private SpriteRenderer rbSprite;
    [Header("Basic arameters")]
    public float speed;
    public float jumpForce;
    private float walkSpeed => speed / 2.5f;//why 2.5?(speed too fast to reach that number)
    private float runSpeed;
    

    private void Awake()
    {
        physicsCheck = GetComponent<PhysicsCheck>();

        inputControl = new PlayerInputControl();
        inputControl.Gameplay.Jump.started += Jump;

        #region ForceToWalk
        runSpeed = speed;
        inputControl.Gameplay.WalkButton.performed += ctx =>
        {
            if (physicsCheck.isGround)
                speed = walkSpeed;
        };
        inputControl.Gameplay.WalkButton.canceled += ctx =>
        {
            if (physicsCheck.isGround)
                speed = runSpeed;
        };
        #endregion

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
        if(physicsCheck.isGround)
          rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }
}
