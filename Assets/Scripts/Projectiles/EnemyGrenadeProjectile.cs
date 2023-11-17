
using UnityEngine;

public class EnemyGrenadeProjectile : MonoBehaviour
{
    PlayerHealth playerHealth;
    PlayerMovement playerMovement;
    public Transform barrel;
    public GameObject explodeIndicator;
    public GameObject explosionObject;
    Rigidbody rigidbody;
    float damage = 25f;
    float timer = 0f;
    float explosionRadius = 5f;
    float explosionForce = 30f;
    int layerMask = ~((1 << 9) | (1 << 11) | (1 << 13));
    // Start is called before the first frame update
    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        rigidbody = GetComponent<Rigidbody>();
        timer = 2.5f;
    }

    private void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        if (timer < 0)
            Explode();
    }

    public void Explode()
    {
        GameObject explosion = Instantiate(explosionObject, transform);
        explosion.GetComponent<ExplosionScript>().EnemyExplode(damage, explosionRadius, explosionForce);
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
