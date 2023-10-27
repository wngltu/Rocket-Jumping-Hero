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
    float bulletSpeed = 500f;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddRelativeForce(new Vector2(0,bulletSpeed));
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
    }
}
