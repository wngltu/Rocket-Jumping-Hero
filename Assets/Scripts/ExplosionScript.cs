using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    //Debugging: if the ExplosionObject is still in scene, that means an explosion failed to go off.
    public GameObject explodeIndicator;
    public GameObject explosiveSFXObject;
    int layerMask = ~((1 << 9) | (1 << 11) | (1 << 13));
    public void PlayerExplode(float damage, float explosionRadius, float explosionForce) //player rocket/explosives
    {
        var cols = Physics.OverlapSphere(this.transform.position, explosionRadius);
        foreach (Collider obj in cols)
        {
            RaycastHit hit;
            Vector3 interactDirection = (obj.gameObject.transform.position - this.transform.position);
            if (Physics.Raycast(transform.position, interactDirection, out hit, explosionRadius, layerMask)) //shoot ray from explosion source
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
                        PlayerMovement playercontroller = obj.GetComponent<PlayerMovement>();
                        playercontroller.AddExplosionForce(transform.position, explosionRadius, explosionForce);
                        print(transform.position);
                    }
                    else if (obj.gameObject.tag == "lever")
                        obj.gameObject.GetComponentInParent<LeverScript>().triggerDoorMaster();
                    else if (obj.gameObject.tag == "explodewhenshot")
                    {
                        if (obj.gameObject.GetComponent<StickyGrenadeProjectile>().exploded == false)
                               obj.gameObject.GetComponent<StickyGrenadeProjectile>().Explode();
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
        newObject.GetComponent<ExplosiveRadius>().explosionRadius = explosionRadius;
        Instantiate(explosiveSFXObject, transform.position, Quaternion.identity);
        Delete();
    }

    public void EnemyExplode(float damage, float explosionRadius, float explosionForce)
    {
        var cols = Physics.OverlapSphere(this.transform.position, explosionRadius);
        foreach (Collider obj in cols)
        {
            RaycastHit hit;
            Vector3 interactDirection = (obj.gameObject.transform.position - this.transform.position);
            if (Physics.Raycast(transform.position, interactDirection, out hit, explosionRadius, layerMask)) //shoot ray from explosion source
            {
                if (hit.collider.gameObject == obj.gameObject) //does explosion have line of sight?
                {
                    if (obj.gameObject.tag == "enemy")
                    {
                        if (obj.isTrigger == true)
                        {

                            Rigidbody rb = obj.GetComponent<Rigidbody>();
                            rb.AddExplosionForce(900f, transform.position, explosionRadius);
                        }
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
        newObject.GetComponent<ExplosiveRadius>().explosionRadius = explosionRadius;
        Instantiate(explosiveSFXObject, transform.position, Quaternion.identity);
        Delete();
    }

    void Delete()
    {
        Destroy(gameObject);
    }
}
