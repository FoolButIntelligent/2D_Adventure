using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("Basic parameters")]
    public float maxHealth;
    public float currentHealth;

    [Header("Damage Invincible")]
    public float invulnerableDuration;
    private float invulnerableCounter;
    public bool invulnerable;

    public UnityEvent<Transform> OnTakeDamage;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            { 
                invulnerable = false;
            }

        }
    }

    public void TakeDamage(Attack attacker) 
    {
        if (invulnerable)
            return;

        //Debug.Log(attacker.damage);
        if (currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
            TriggerInvulnerable();
            //hurt
            OnTakeDamage?.Invoke(attacker.transform);
        }
        else
        {
            currentHealth = 0;
            //death trigger
        }
    }

    /// <summary>
    /// invulnerable trigger
    /// </summary>
    private void TriggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }
}
