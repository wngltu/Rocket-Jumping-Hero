using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public static Enemy Instance;
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    public float maxVelocityY = 7f;
    public float maxVelocityX = 12.5f;

    public PrefabLoot prefabLoot;
    public Slider healthBar;
    protected Rigidbody rb;
    public PlayerMovement player;
    public AIPath aiPath;
    public GameObject weaponLoot;
    public GameObject damageIndicator;
    public GameObject deathSoundObject;

    public bool Died = false; //bool to make sure drops dont duplicate
    public bool aggroed = false;
    public bool playerToTheRight; //which direction in reference to the game object is the player currently?
    public bool attacking = false;
    public bool isBoss = false;


    // Start is called before the first frame update
    protected void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        prefabLoot = FindObjectOfType<PrefabLoot>();
        rb = GetComponent<Rigidbody>();
        aiPath = GetComponent<AIPath>();
        Instance = this;
        currentHealth = maxHealth;
        healthBar.value = currentHealth / maxHealth;
    }

    protected void Update()
    {
        
        if (rb.velocity.y > maxVelocityY && !attacking)
            rb.velocity = new Vector3(rb.velocity.x, maxVelocityY, rb.velocity.z);
        else if (rb.velocity.y < -maxVelocityY && !attacking)
            rb.velocity = new Vector3(rb.velocity.x, -maxVelocityY, rb.velocity.z);

        if (rb.velocity.x > maxVelocityX && !attacking)
            rb.velocity = new Vector3(maxVelocityX, rb.velocity.y, rb.velocity.z);
        else if (rb.velocity.x < -3.5f && !attacking)
            rb.velocity = new Vector3(-maxVelocityX, rb.velocity.y, rb.velocity.z);
        
    }

    public void takeDamage(float damage)
    {
        GameObject damageNumber = Instantiate(damageIndicator, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);
        damageNumber.GetComponentInChildren<DamageNumberScript>().damage = damage;
        currentHealth -= damage;
        healthBar.value = currentHealth / maxHealth;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        if (Died != true)
        {
            if (isBoss)
            {
                FindObjectOfType<Level1FSM>().bossKilled = true;
            }
            GameObject loot = Instantiate(weaponLoot, transform.position, Quaternion.identity, null);
            loot.gameObject.transform.SetParent(null);
            Died = true;
            Instantiate(deathSoundObject, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    protected virtual void UpdatePlayerDirection()
    {
        if (player.gameObject.transform.position.x > this.gameObject.transform.position.x)
        {
            playerToTheRight = true;
        }
        else if (player.gameObject.transform.position.x < this.gameObject.transform.position.x)
        {
            playerToTheRight = false;
        }
    }

    protected virtual Vector3 GetVectorToPlayer()
    {
        return player.gameObject.transform.position - gameObject.transform.position;
    }
}
