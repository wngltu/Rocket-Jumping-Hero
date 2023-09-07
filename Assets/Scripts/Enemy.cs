using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Enemy Instance;
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void takeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    void Die()
    {
        Destroy(this.gameObject);
    }
}
