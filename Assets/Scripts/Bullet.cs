using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Enemy enemyScript;
    public static Bullet Instance;
    public Transform barrel;
    Rigidbody rigidbody;
    float damage = 25f;
    float timer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddRelativeForce(new Vector2(0,900));
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
        if (other.tag == "enemy")
        {
            enemyScript = other.GetComponent<Enemy>();
            enemyScript.takeDamage(damage);
            Destroy(this.gameObject);
        }
    }
}
