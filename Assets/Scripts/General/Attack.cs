using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage;
    public float attackRange;
    public float attackRate;

    private void OnTriggerStay2D(Collider2D collision)
    {
        //?'s meaing:if have Character component,excute it.
        collision.GetComponent<Character>()?.TakeDamage(this);
    }
}
