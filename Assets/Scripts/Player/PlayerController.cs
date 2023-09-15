using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;
    public Vector2 inputDirection;
    public float speed;
    public Rigidbody2D rb;
    private SpriteRenderer rbSprite;

    private void Awake()
    {
        inputControl = new PlayerInputControl(); 
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

        //人物面朝方向

        if (inputDirection.x > 0)
            rbSprite.flipX = false;
        if(inputDirection.x < 0)
            rbSprite.flipX = true;
        //人物翻转
    }
}
