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
    private CapsuleCollider2D coll;
    private SpriteRenderer rbSprite;
    [Header("Basic arameters")]
    public float speed;
    public float jumpForce;
    private float walkSpeed => speed / 2.5f;//why 2.5?(speed too fast to reach that number)
    private float runSpeed;

    public bool isCrouch;
    private Vector2 originalOffset;
    private Vector2 originalSize;

    public float hurtForce;
    public bool isHurt;
    public bool isDead;

    private void Awake()
    {
        physicsCheck = GetComponent<PhysicsCheck>();
        rbSprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<CapsuleCollider2D>();

        inputControl = new PlayerInputControl();
        inputControl.Gameplay.Jump.started += Jump;

        originalOffset = coll.offset;
        originalSize = coll.size;

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
        if (!isHurt)
        {
            Move();
        }  
    }

    public void Move()
    {
        //character move
        if(!isCrouch)
          rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime,rb.velocity.y);

        //character flip

        if (inputDirection.x > 0)
            rbSprite.flipX = false;
        if(inputDirection.x < 0)
            rbSprite.flipX = true;

        //crouch
        isCrouch = inputDirection.y < -0.5f && physicsCheck.isGround;
        if (isCrouch)
        {
            //change collider size
            coll.offset = new Vector2(-0.05f, 0.85f);
            coll.size = new Vector2(0.7f, 1.7f);
        }
        else
        { 
            //recover collider size
            coll.size = originalSize;
            coll.offset = originalOffset;
        }
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        //Debug.Log("JUMP");
        if(physicsCheck.isGround)
          rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    public void GetHurt(Transform attaker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(transform.position.x - attaker.position.x, 0).normalized;
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse); 
    }

    public void PlayerDead()
    {
        isDead = true;
        inputControl.Gameplay.Disable();
    }

    //Avoid attack by enemy when player dead
    private void CheckState()
    {
        if (isDead)
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        else
            gameObject.layer = LayerMask.NameToLayer("Player");
    }
}
