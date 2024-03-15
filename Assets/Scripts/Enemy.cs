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
    public GameObject model;
    public SphereCollider corpseCollider;
    public GameObject explosionVFX;

    public bool Died = false; //bool to make sure drops dont duplicate
    public bool aggroed = false;
    public bool playerToTheRight; //which direction in reference to the game object is the player currently?
    public bool attacking = false;
    public bool isBoss = false;
    public bool isInvincible = false;


    // Start is called before the first frame update
    protected void Start()
    {
        if (GetComponent<SphereCollider>() != null)
            corpseCollider = GetComponent<SphereCollider>();
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

    public virtual void takeDamage(float damage)
    {
        if (isInvincible)
        {
            damage = 0;
        }
        GameObject damageNumber = Instantiate(damageIndicator, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);
        damageNumber.GetComponentInChildren<DamageNumberScript>().damage = damage;
        if (damage > 0)
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
            foreach (Collider col in this.gameObject.GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }
            corpseCollider.enabled = true;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(Random.Range(-100f, 100f), Random.Range(100f, 1000f), 0);
            rb.AddTorque(Random.Range(-50f, 50f), Random.Range(-50f, 50f), Random.Range(-50f, 50f));
            healthBar.gameObject.SetActive(false);
            
            if (isBoss)
            {
                if (FindAnyObjectByType<Level1FSM>() != null)
                    FindObjectOfType<Level1FSM>().bossKilled = true;
                if (FindAnyObjectByType<Level2FSM>() != null)
                    FindAnyObjectByType<Level2FSM>().bossKilled = true;
            }
            GameObject loot = Instantiate(weaponLoot, transform.position, Quaternion.identity, null);
            loot.gameObject.transform.SetParent(null);
            loot.name = loot.name.Replace("(Clone)", "");
            Died = true;
            Invoke("ExplodeCorpse", 1f);
        }
    }

    protected virtual void ExplodeCorpse()
    {
        Instantiate(explosionVFX, transform.position, Quaternion.identity, null);
        Instantiate(deathSoundObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
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
