using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySingleSpawnerScript : MonoBehaviour
{
    public GameObject enemyToSpawn;
    void Start()
    {
        GameObject newobj = Instantiate(enemyToSpawn, transform);
        newobj.transform.parent = null;
        Destroy(gameObject);
    }
}
