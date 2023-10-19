using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    Enemy enemyScript;
    public static Rocket Instance;
    public Transform barrel;
    public GameObject explodeIndicator;
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
        if (collision.gameObject.tag == "enemy" || collision.gameObject.tag == "door" || collision.gameObject.tag == "ground" || collision.gameObject.tag == "platform")
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
                        if (obj.isTrigger == false)
                        {
                            Rigidbody rb = obj.GetComponent<Rigidbody>();
                            rb.AddExplosionForce(900f, transform.position, explosionRadius);
                            obj.GetComponent<Enemy>().takeDamage(damage * (explosionRadius - (this.transform.position - obj.transform.position).magnitude) / 5);
                        }
                    }
                    else if (obj.gameObject.tag == "Player")
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
                    else if (obj.gameObject.tag == "lever")
                        obj.gameObject.GetComponentInParent<LeverScript>().triggerDoorMaster();
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
