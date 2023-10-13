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

    public PrefabLoot prefabLoot;
    public Slider healthBar;
    protected Rigidbody rb;
    public PlayerMovement player;
    public AIPath aiPath;

    public bool Died = false; //bool to make sure drops dont duplicate
    public bool aggroed = false;

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

    public void takeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.value = currentHealth / maxHealth;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void AggroEnemy()
    {
        aggroed = true;
        aiPath.enabled = true;
    }

    public void DeaggroEnemy()
    {
        aggroed = false;
        aiPath.enabled = false;
    }
    protected virtual void Die()
    {
        if (Died != true)
        {
            GameObject loot = Instantiate(prefabLoot.meleeDrop, this.transform);
            loot.gameObject.transform.SetParent(null);
            Died = true;
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (aggroed == false)
                AggroEnemy();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (aggroed == true)
                DeaggroEnemy();
        }
    }
}
