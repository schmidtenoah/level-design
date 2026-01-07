
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] private int healthPoints;
    [SerializeField] private Text healthText;
    
    private void OnEnable()
    {
        PlayerHurtbox.OnVirusHitPlayer += TakeDamage;
    }

    private void OnDisable()
    {
        PlayerHurtbox.OnVirusHitPlayer -= TakeDamage;
    }
    
    private void UpdateHealthText()
    {
        healthText.text = $"Health: {healthPoints}";
    }

    private void TakeDamage()
    {
        --healthPoints;
        UpdateHealthText();

        if (healthPoints <= 0)
        {
            Debug.Log("Player Dead!");
        }
    }
    private void Start()
    {
        UpdateHealthText();
    }
}
