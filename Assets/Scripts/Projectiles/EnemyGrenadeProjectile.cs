
using UnityEngine;

public class EnemyGrenadeProjectile : MonoBehaviour
{
    PlayerHealth playerHealth;
    PlayerMovement playerMovement;
    public Transform barrel;
    public GameObject explodeIndicator;
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
        var cols = Physics.OverlapSphere(this.transform.position, explosionRadius);
        foreach (Collider obj in cols)
        {
            RaycastHit hit;
            Vector3 interactDirection = (obj.gameObject.transform.position - this.transform.position);
            if (Physics.Raycast(transform.position, interactDirection, out hit, explosionRadius, layerMask)) //shoot ray from barrel of gun
            {
                if (hit.collider.gameObject == obj.gameObject) //does explosion have line of sight?
                {
                    if (obj.gameObject.tag == "enemy")
                    {
                        if (obj.isTrigger == true)
                        {

                            Rigidbody rb = obj.GetComponent<Rigidbody>();
                            rb.AddExplosionForce(900f, transform.position, explosionRadius);                        }
                    }
                    else if (obj.gameObject.tag == "Player")
                    {
                        if (obj.isTrigger == false)
                        {
                            PlayerMovement playercontroller = obj.GetComponent<PlayerMovement>();
                            playercontroller.AddExplosionForce(transform.position, explosionRadius, explosionForce);
                            obj.GetComponent<PlayerHealth>().TakeDamage(damage * (explosionRadius - (this.transform.position - obj.transform.position).magnitude) / 5);
                        }
                    }
                    else if (obj.GetComponent<Rigidbody>() != null) //component must have rigidbody to be displaced
                    {
                        Rigidbody rb = obj.GetComponent<Rigidbody>();
                        rb.AddExplosionForce(900f, transform.position, explosionRadius);
                    }
                }
            }
        }
        GameObject newObject = Instantiate(explodeIndicator, transform.position, Quaternion.identity); //spawn a circle showing blast radius
        newObject.GetComponent<ExplosiveRadius>().explosionRadius = this.explosionRadius;
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
