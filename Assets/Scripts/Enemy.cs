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

    // Start is called before the first frame update
    void Start()
    {
        prefabLoot = FindObjectOfType<PrefabLoot>();
        Instance = this;
        currentHealth = maxHealth;
        healthBar.value = currentHealth / maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

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
    void Die()
    {
        print("die");
        GameObject loot = Instantiate(prefabLoot.pistolDrop, this.transform);
        loot.gameObject.transform.SetParent(null);
        Destroy(this.gameObject);
    }
}
