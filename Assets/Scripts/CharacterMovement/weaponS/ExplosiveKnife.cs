using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveKnife : weaponScript
{
    public GameObject explodeIndicator;
    public float explosionRadius = 5f;
    public float range = 2f;
    public float explosionForce = 20f;
    public GameObject grenadeVisual;
    Vector2 interactDirection;

    int layerMask = ~((1 << 3) | (1 << 8) | (1 << 9) | (1 << 11) | (1 << 13));
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        maxMag = 1;
        maxReserve = 0;
        currentMag = 1;
        currentReserve = 0;
        baseDamage = 50f;
        explosionForce = 20f;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Mouse0) && pauseManager.paused == false && currentMag > 0)
        {
            if (canShoot)
                Shoot();
        }
        if (Input.GetKeyDown(KeyCode.R) && pauseManager.paused == false && currentMag != maxMag)
            Reload();
    }

    public void Shoot()
    {
        interactDirection = playerCam.ScreenToWorldPoint(Input.mousePosition) - playerCam.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, interactDirection, out hit, range, layerMask)) //shoot ray from barrel of gun
        {
            Debug.DrawRay(transform.position, interactDirection);
            Debug.Log(hit.collider.gameObject.name.ToString());

            var cols = Physics.OverlapSphere(hit.point, explosionRadius);
            foreach (Collider obj in cols)
            {
                if (obj.gameObject.tag == "enemy")
                {
                    if (obj.isTrigger == false)
                    {
                        Rigidbody rb = obj.GetComponent<Rigidbody>();
                        rb.AddExplosionForce(900f, hit.point, explosionRadius);
                        obj.GetComponent<Enemy>().takeDamage(baseDamage * (explosionRadius - (this.transform.position - obj.transform.position).magnitude) / 5);
                    }
                }
                else if (obj.gameObject.tag == "Player")
                {
                    PlayerMovement playercontroller = obj.GetComponent<PlayerMovement>();
                    playercontroller.AddExplosionForce(hit.point, explosionRadius, explosionForce);
                    print(transform.position);
                }
            }
            if (hit.collider != null) //if did not miss
            {
                GameObject newObject = Instantiate(explodeIndicator, hit.point, Quaternion.identity); //spawn a circle showing blast radius
                newObject.GetComponent<ExplosiveRadius>().explosionRadius = this.explosionRadius;
                currentMag--;
                grenadeVisual.SetActive(false);
            }
        }
        UpdateHUDValues();
    }
}
