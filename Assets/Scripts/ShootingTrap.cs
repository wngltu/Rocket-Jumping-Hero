using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShootingTrap : MonoBehaviour
{
    float shootTimer;
    float shootCooldown = 2f;
    float baseDamage = 5f;
    float windUpTime = .75f;

    bool isWinding = false;

    public GameObject barrel;
    public GameObject enemyBullet;
    public GameObject barrelGraphic;
    SpriteRenderer barrelSpriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        shootTimer = shootCooldown;
        barrelSpriteRenderer = barrelGraphic.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shootTimer > 0 && !isWinding)
            shootTimer -= Time.deltaTime;
        else if (shootTimer < 0 && !isWinding)
            shootTimer = 0;

        if (shootTimer == 0)
        {
            WindUp();
            shootTimer = shootCooldown;
        }
    }

    void WindUp()
    {
        isWinding = true;
        barrelSpriteRenderer.material.color = Color.red;
        Invoke("Shoot", windUpTime);
    }

    void Shoot()
    {
        isWinding = false;
        barrelSpriteRenderer.material.color = Color.white;
        GameObject newBullet = Instantiate(enemyBullet, barrel.transform.position, barrel.transform.rotation);
        newBullet.GetComponent<EnemyBullet>().damage = baseDamage;
    }
}
