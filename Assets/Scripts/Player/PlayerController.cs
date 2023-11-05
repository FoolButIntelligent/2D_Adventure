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
    private PlayerAnimation playerAnimation;
    private Character character;
    [Header("Basic Parameters")]
    public float speed;
    public float jumpForce;
    public float walljumpForce;
    public float hurtForce;
    private float walkSpeed => speed / 2.5f;//why 2.5?(speed too fast to reach that number)
    private float runSpeed;
    public float slideDistance;
    public float slideSpeed;
    public int slidePowerCost;

    private Vector2 originalOffset;
    private Vector2 originalSize;
    [Header("Physics Material")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;
    [Header("State")]
    public bool isCrouch;
    public bool isHurt;
    public bool isDead;
    public bool isAttack;
    public bool wallJump;
    public bool isSlide;

    private void Awake()
    {
        physicsCheck = GetComponent<PhysicsCheck>();
        coll = GetComponent<CapsuleCollider2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
        character = GetComponent<Character>();

        inputControl = new PlayerInputControl();

        originalOffset = coll.offset;
        originalSize = coll.size;

        //jump
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

        //attack
        inputControl.Gameplay.Attack.started += PlayerAttack;
        //slide
        inputControl.Gameplay.Slide.started += Slide;
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
        CheckState();
    }

    private void FixedUpdate()
    {
        if (!isHurt && !isAttack)
        {
            Move();
        }  
    }

    public void Move()
    {
        //character move
        if (!isCrouch && !wallJump)
          rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime,rb.velocity.y);

        //character flip

        if (inputDirection.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        if (inputDirection.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);

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
        if (physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            isSlide = false;
            StopAllCoroutines();
        }
          
        else if (physicsCheck.onWall)
        {
            rb.AddForce(new Vector2(-inputDirection.x, 2.5f) * walljumpForce, ForceMode2D.Impulse);
            wallJump = true;
        }

    }


    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        playerAnimation.PlayerAttack();
        isAttack = true;
    }


    private void Slide(InputAction.CallbackContext obj)
    {
        if (!isSlide && physicsCheck.isGround && character.currentPower >= slidePowerCost)
        {
            isSlide = true;
            //gameObject.layer = LayerMask.NameToLayer("Enemy");
            Debug.Log("sliding");
            var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x, transform.position.y);

            StartCoroutine(TriggerSlide(targetPos));

            character.OnSlide(slidePowerCost);
        }

        
    }

    private IEnumerator TriggerSlide(Vector3 target)
    {
        do
        {
            yield return null;
            if (!physicsCheck.isGround)
                break;

            if (physicsCheck.touchLeftWall && transform.localScale.x < 0f || physicsCheck.touchRightWall && transform.localScale.x > 0f)
            {
                isSlide = false;
                break;
            }
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed, transform.position.y));
        }while (MathF.Abs(target.x - transform.position.x) > 0.1f);

        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    #region UnityEvent
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
    #endregion

    //Avoid attack by enemy when player dead && physics material check
    private void CheckState()
    {
        if (isDead)
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        else
            gameObject.layer = LayerMask.NameToLayer("Player");

        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;

        if (physicsCheck.onWall)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }

        if (wallJump && rb.velocity.y < 0f)
        {
            wallJump = false;
        }
    }
}
