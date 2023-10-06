using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Knife : weaponScript
{
    public GameObject explodeIndicator;
    public float explosionRadius = .35f;
    public float range = 2f;
    Vector2 interactDirection;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        maxMag = 1;
        maxReserve = 0;
        currentMag = 1;
        currentReserve = 0;
        baseDamage = 25;
    }
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
        if (Physics.Raycast(transform.position, interactDirection, out hit, range)) //shoot ray from barrel of gun
        {
            Debug.DrawRay(transform.position, interactDirection);
            Debug.Log(hit.collider.gameObject.name.ToString());

            if (hit.collider.gameObject.CompareTag("enemy"))
            {
                hit.collider.gameObject.GetComponent<Enemy>().takeDamage(baseDamage);
                GameObject newExplosiveIndicator = Instantiate(explodeIndicator, hit.point, Quaternion.identity);
                newExplosiveIndicator.GetComponent<ExplosiveRadius>().explosionRadius = .5f;
            }
        }
        //GameObject clone = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
        UpdateHUDValues();
    }

}
