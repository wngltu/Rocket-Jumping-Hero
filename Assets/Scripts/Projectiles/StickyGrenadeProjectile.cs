using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyGrenadeProjectile : MonoBehaviour
{
    Enemy enemyScript;
    public static StickyGrenadeProjectile Instance;
    public Transform barrel;
    public GameObject explodeIndicator;
    public GameObject explosionObject;
    Rigidbody rigidbody;
    float damage = 50f;
    float timer = 0f;
    float explosionRadius = 5f;
    float explosionForce = 30f;
    int layerMask = ~((1 << 9) | (1 << 11) | (1 << 13));

    public bool exploded = false;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddRelativeForce(new Vector3(0, 100, -300));
    }
    public void Explode()
    {
        exploded = true;
        GameObject newObj = Instantiate(explosionObject, transform.position, Quaternion.identity);
        newObj.GetComponent<ExplosionScript>().PlayerExplode(damage, explosionRadius, explosionForce);
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            rigidbody.isKinematic = true;
            this.gameObject.transform.SetParent(collision.transform, true);
            this.gameObject.GetComponent<SphereCollider>().enabled = false;
        }
        else
        {
            rigidbody.isKinematic = true;
            this.gameObject.GetComponent<SphereCollider>().enabled = false;
            this.gameObject.GetComponent<BoxCollider>().enabled = true;
        }
    }
}
