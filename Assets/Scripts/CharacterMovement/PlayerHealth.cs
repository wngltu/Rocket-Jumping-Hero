using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth = 100;

    public TextMeshProUGUI currentHealthText;
    public TextMeshProUGUI maxHealthText;
    public Slider healthSlider;
    public Camera playerCamera;

    public PauseMenu pauseMenuScript;
    public GameObject gameOverScreen;

    private void Start()
    {
        pauseMenuScript = FindObjectOfType<PauseMenu>();
        currentHealth = maxHealth;
        UpdateHealth();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            TakeDamage(15);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealth();
        if (currentHealth < 0)
        { 
            currentHealth = 0;
            UpdateHealth();
        }
        if (currentHealth == 0)
            Die();
    }

    public void UpdateHealth()
    {
        currentHealthText.text = currentHealth.ToString();
        healthSlider.value = currentHealth / maxHealth;
    }

    public void Die()
    {
        pauseMenuScript.Pause();
        pauseMenuScript.pauseMenu.SetActive(false);
        pauseMenuScript.enabled = false;
        gameOverScreen.SetActive(true);
        this.gameObject.GetComponent<PlayerMovement>().enabled = false;
    }
}
