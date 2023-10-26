using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    bool alive;
    public GameObject enemyPrefab;
    float respawnCooldown = 1.5f;
    float respawnTimer;
    // Start is called before the first frame update
    void Start()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform);
        enemy.transform.SetParent(transform);
        enemy.GetComponent<MeleeGruntFSM>().prefabLoot = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (respawnTimer > 0)
            respawnTimer -= Time.deltaTime;
        else if (respawnTimer < 0)
            respawnTimer = 0;

        if (transform.childCount == 0 && respawnTimer == 0)
        {
            GameObject enemy = Instantiate(enemyPrefab, transform);
            enemy.transform.SetParent(transform);
            enemy.GetComponent<MeleeGruntFSM>().prefabLoot = null;
            respawnTimer = respawnCooldown;
        }
    }
}
