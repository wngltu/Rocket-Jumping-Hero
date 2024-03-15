using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class healthPackScript : MonoBehaviour
{
    PlayerHealth playerHealthScript;
    PlayerInteraction playerInteractionScript;
    public TextMeshProUGUI errorText;

    private void Start()
    {
        playerHealthScript = FindObjectOfType<PlayerHealth>();
        playerInteractionScript = FindObjectOfType<PlayerInteraction>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && playerHealthScript.currentHealth < playerHealthScript.maxHealth)
        {
            if (playerHealthScript.currentHealth + 25 > playerHealthScript.maxHealth)
            {
                playerHealthScript.currentHealth = playerHealthScript.maxHealth;
                playerHealthScript.UpdateHealth();
            }
            else
            {
                playerHealthScript.currentHealth += 25;
                playerHealthScript.UpdateHealth();
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Player") && playerHealthScript.currentHealth >= playerHealthScript.maxHealth)
        {
            playerInteractionScript.DisplayFeedbackText("Cannot pick up health pack, player is at max health.");
        }
    }
}
