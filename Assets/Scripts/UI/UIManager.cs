using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public PlayerStatBar playerStatBar;
    [Header("Event Lisener")]
    public CharacterEventSO healthEvent;

    private void OnEnable()
    {
        healthEvent.OnEventsRaised += OnHealthEvent;
    }

    private void OnDisable()
    {
        healthEvent.OnEventsRaised -= OnHealthEvent;
    }

    private void OnHealthEvent(Character character)
    {
        var percentage = character.currentHealth / character.maxHealth;
        playerStatBar.OnHealthChange(percentage);
        playerStatBar.OnPowerChange(character);
    }
}
