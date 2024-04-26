using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth = 100;
    public float invincibleTime = .75f;
    public float trapInvincibleTime = 1.5f;
    public float healthRectMaxWidth;
    public float healthRectMaxHeight;
    public bool isInvincible = false;
    public bool isTrapInvincible = false;
    public bool isDead = false;

    public TextMeshProUGUI currentHealthText;
    public TextMeshProUGUI maxHealthText;
    public Slider healthSlider;
    public Camera playerCamera;
    public RectTransform healthRectFill;
    public Canvas healthCanvas;

    public PauseMenu pauseMenuScript;
    public GameObject gameOverScreen;
    public GameObject damageIndicatorPrefab;
    public GameObject model; 
    public AudioSource deathSound;
    public AudioSource hurtSound;
    public AudioSource hurtInvincibleSound;
    public Enemy[] enemyList;
    public Animation invincibilityFlash;
    CharacterController controller;
    Rigidbody rb;

    private void Start()
    {
        healthRectMaxWidth = healthRectFill.rect.width;
        healthRectMaxHeight = healthRectFill.rect.height;
        pauseMenuScript = FindObjectOfType<PauseMenu>();
        currentHealth = maxHealth;
        UpdateHealth();
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
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
        if (!isInvincible && damage > 0)
        {
            ApplyInvincibility();
            currentHealth -= damage;
            Instantiate(damageIndicatorPrefab, healthRectFill.transform);
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
        if (!isTrapInvincible && damage > 0)
        {
            ApplyTrapInvincibility();
            currentHealth -= damage;
            Instantiate(damageIndicatorPrefab, healthRectFill.transform);
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
        Instantiate(damageIndicatorPrefab, healthRectFill.transform);
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
        invincibilityFlash.Stop();
        invincibilityFlash.Play();
        isInvincible = true;
        Invoke("RemoveInvincibility", invincibleTime);
    }
    void RemoveInvincibility()
    {
        isInvincible = false;   
    }
    void ApplyTrapInvincibility()
    {
        invincibilityFlash.Stop();
        invincibilityFlash.Play();
        isTrapInvincible = true;
        Invoke("RemoveTrapInvincibility", trapInvincibleTime);    
    }
    void RemoveTrapInvincibility()
    {
        isTrapInvincible = false;
    }

    public void UpdateHealth()
    {
        Debug.Log("Player Health Changed");
        currentHealthText.text = ((int)currentHealth).ToString();
        healthSlider.value = currentHealth / maxHealth;
        healthRectFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentHealth / maxHealth * healthRectMaxWidth);
    }

    public void Die()
    {
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            enemy.Died = true;
        }
        controller.enabled = false;
        rb.isKinematic = false;
        isDead = true;
        deathSound.Play();
        model.SetActive(false);
        //pauseMenuScript.Pause();
        pauseMenuScript.pauseMenu.SetActive(false);
        pauseMenuScript.enabled = false;
        gameOverScreen.SetActive(true);
        this.gameObject.GetComponent<PlayerMovement>().enabled = false;
        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        this.gameObject.GetComponent<weaponManager>().DropCurrentWeapon();
        this.gameObject.GetComponent<weaponManager>().enabled = false;
        this.gameObject.GetComponent<PlayerInteraction>().enabled = false;
    }
}
