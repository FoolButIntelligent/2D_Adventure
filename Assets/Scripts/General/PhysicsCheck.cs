using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    private CapsuleCollider2D coll;
    private PlayerController playerController;
    private Rigidbody2D rb;

    [Header("Check Parameters")]
    public bool manual;
    public bool isPlayer;
    public Vector2 bottomOffset;//�ŵ�λ�Ʋ�ֵ
    public Vector2 leftOffset;
    public Vector2 rightOffset; 
    public float checkRadius;
    public LayerMask groundLayer;

    [Header("State")]
    public bool isGround;
    public bool touchLeftWall;
    public bool touchRightWall;
    public bool onWall;

    private void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (!manual)
        {
            rightOffset = new Vector2(( coll.bounds.size.x + coll.offset.x) / 2, coll.bounds.size.y / 2);
            leftOffset = new Vector2(-rightOffset.x, rightOffset.y);
        }

        if (isPlayer)
            playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        Check();
    }

    public void Check()
    {
       //check ground
       if(onWall)
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRadius,groundLayer);
       else
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, 0), checkRadius, groundLayer);
        //detect wall
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(leftOffset.x, leftOffset.y), checkRadius, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(rightOffset.x, rightOffset.y), checkRadius, groundLayer);

        //on wall
        if(isPlayer)
            onWall = (touchLeftWall && playerController.inputDirection.x < 0f ||
                      touchRightWall && playerController.inputDirection.x > 0f) && rb.velocity.y < 0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(leftOffset.x, leftOffset.y), checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(rightOffset.x, rightOffset.y), checkRadius);
    }
}
