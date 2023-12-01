using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    Enemy enemyScript;
    public static Rocket Instance;
    public Transform barrel;
    public GameObject explodeIndicator;
    public GameObject explosionObject;
    Rigidbody rigidbody;
    float damage = 50f;
    float timer = 0f;
    float explosionRadius = 5f;
    float explosionForce = 30f;
    int layerMask = ~((1 << 9) | (1 << 11) | (1 << 13));
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

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name.ToString());
        if (collision.gameObject.tag == "enemy" || 
            collision.gameObject.tag == "door" ||
            collision.gameObject.tag == "rdoor" ||
            collision.gameObject.tag == "ground" || 
            collision.gameObject.tag == "platform" || 
            collision.gameObject.tag == "droppedweapon")
            Explode();
    }

    public void Explode()
    {
        GameObject explosion = Instantiate(explosionObject, transform);
        explosion.GetComponent<ExplosionScript>().PlayerExplode(damage, explosionRadius, explosionForce);
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
