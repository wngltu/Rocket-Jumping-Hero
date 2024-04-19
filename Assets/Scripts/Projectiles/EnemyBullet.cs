using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    PlayerHealth playerHealth;
    public static EnemyBullet Instance;
    public Transform barrel;
    Rigidbody rigidbody;
    public float damage = 15f;
    float timer = 0f;
    public float bulletSpeed = 500f;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddRelativeForce(new Vector3(0, 0, -bulletSpeed));
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > 1.5f)
        {
            Destroy(this.gameObject);
        }
    }
    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerHealth.TakeDamage(damage);
            Destroy(this.gameObject);
        }
        else if (other.tag == "ground" || other.tag == "door")
        {
            Destroy(this.gameObject);
        }
    }
}
