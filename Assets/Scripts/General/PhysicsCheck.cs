using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    [Header("Check Parameters")]
    public Vector2 bottomOffset;//�ŵ�λ�Ʋ�ֵ
    public float checkRadius;
    public LayerMask groundLayer;
    [Header("State")]
    public bool isGround;
    private void Update()
    {
        Check();
    }

    public void Check()
    {
       //������
       isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset , checkRadius,groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRadius);
    }
}
