using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth = 100;
    public float invincibleTime = .75f;
    public float trapInvincibleTime = 1.5f;
    public bool isInvincible = false;
    public bool isTrapInvincible = false;

    public TextMeshProUGUI currentHealthText;
    public TextMeshProUGUI maxHealthText;
    public Slider healthSlider;
    public Camera playerCamera;

    public PauseMenu pauseMenuScript;
    public GameObject gameOverScreen;
    public GameObject invincibilityShield;
    public AudioSource deathSound;
    public AudioSource hurtSound;
    public AudioSource hurtInvincibleSound;

    private void Start()
    {
        pauseMenuScript = FindObjectOfType<PauseMenu>();
        currentHealth = maxHealth;
        UpdateHealth();
        invincibilityShield.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            TakeDamage(15);
        if (Input.GetKeyDown(KeyCode.O))
            TakeTrueDamage(15);
    }

    public void TakeDamage(float damage) //used by general damage sources, like enemies and enemy projectiles
    {
        if (!isInvincible)
        {
            ApplyInvincibility();
            currentHealth -= damage;
            UpdateHealth();
            if (currentHealth < 0)
            {
                currentHealth = 0;
                UpdateHealth();
            }
            if (currentHealth == 0)
                Die();
            else
                hurtSound.Play();
        }
    }

    public void TakeTrapDamage(float damage) //used by traps, should have a separate trap invincibility timer
    {
        if (!isTrapInvincible)
        {
            ApplyTrapInvincibility();
            currentHealth -= damage;
            UpdateHealth();
            if (currentHealth < 0)
            {
                currentHealth = 0;
                UpdateHealth();
            }
            if (currentHealth == 0)
                Die();
            else
                hurtSound.Play();
        }
    }

    public void TakeTrueDamage(float damage) //used by killboxes, should bypass invinciblity
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
        else
            hurtSound.Play();
    }

    void ApplyInvincibility()
    {
        isInvincible = true;
        Invoke("RemoveInvincibility", invincibleTime);
        invincibilityShield.SetActive(true);
    }
    void RemoveInvincibility()
    {
        isInvincible = false;
        invincibilityShield.SetActive(false);
    }
    void ApplyTrapInvincibility()
    {
        isTrapInvincible = false;
        Invoke("RemoveTrapInvincibility", trapInvincibleTime);
        invincibilityShield.SetActive(true);
    }
    void RemoveTrapInvincibility()
    {
        isTrapInvincible = false;
        invincibilityShield.SetActive(false);
    }

    public void UpdateHealth()
    {
        currentHealthText.text = ((int)currentHealth).ToString();
        healthSlider.value = currentHealth / maxHealth;
    }

    public void Die()
    {
        deathSound.Play();
        pauseMenuScript.Pause();
        pauseMenuScript.pauseMenu.SetActive(false);
        pauseMenuScript.enabled = false;
        gameOverScreen.SetActive(true);
        this.gameObject.GetComponent<PlayerMovement>().enabled = false;
        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
}
