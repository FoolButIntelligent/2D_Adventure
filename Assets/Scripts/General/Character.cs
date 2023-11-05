using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("Basic parameters")]
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;

    [Header("Damage Invincible")]
    public float invulnerableDuration;
    [HideInInspector]public float invulnerableCounter;
    public bool invulnerable;

    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;

    private void Start()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }

    private void Update()
    {
        if(invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            { 
                invulnerable = false;
            }

        }

        if(currentPower < maxPower)
        {
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            //death/refresh health
            currentHealth = 0;
            OnHealthChange?.Invoke(this);
            OnDie?.Invoke();
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
            OnDie?.Invoke();
        }

        OnHealthChange?.Invoke(this);
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

    public void OnSlide(int cost)
    {
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }
}
