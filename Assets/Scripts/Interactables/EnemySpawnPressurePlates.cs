using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPressurePlates : MonoBehaviour
{
    EnemySpawner enemySpawner;
    public bool interactable = true;
    // Start is called before the first frame update
    void Start()
    {
        enemySpawner = GetComponentInParent<EnemySpawner>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            enemySpawner.SpawnEnemy();
            activatePlate();
            Invoke("deactivatePlate", 3f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (interactable)
            {
                enemySpawner.SpawnEnemy();
            }
        }
    }

    public void activatePlate()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / 2, transform.localScale.z);
        setUninteractable();
    }

    public void deactivatePlate()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 2, transform.localScale.z);
    }

    void setUninteractable()
    {
        interactable = false;
        Invoke("setInteractable", 3f);
    }
    void setInteractable()
    {
        interactable = true;
        deactivatePlate();
    }
}
