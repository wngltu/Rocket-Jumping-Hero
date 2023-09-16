using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    Enemy enemyScript;
    public static Rocket Instance;
    public Transform barrel;
    Rigidbody rigidbody;
    float damage = 50f;
    float timer = 0f;
    float explosionRadius = 5f;
    float explosionForce = 30f;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddRelativeForce(new Vector2(0, 900));
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
            Explode();
        }
        if (other.tag == "ground")
        {
            Explode();
        }
    }

    public void Explode()
    {
        var cols = Physics.OverlapSphere(this.transform.position, 5f);
        foreach (Collider obj in cols)
        {
            if (obj.gameObject.tag == "enemy")
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                rb.AddExplosionForce(900f, transform.position, explosionRadius);
            }
            if (obj.gameObject.tag == "Player")
            {
                /*
                CharacterController cc = obj.GetComponent<CharacterController>();
                cc.enabled = false;
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.AddExplosionForce(900f, transform.position, explosionRadius);
                cc.enabled = true;
                cc.
                */
                PlayerMovement playercontroller = obj.GetComponent<PlayerMovement>();
                playercontroller.AddExplosionForce(transform.position, explosionRadius, explosionForce);
                print(transform.position);
            }
        }
        Delete();
    }
    void Delete()
    {
        Destroy(this.gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
}
