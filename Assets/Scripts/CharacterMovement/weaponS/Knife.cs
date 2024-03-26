using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Knife : weaponScript
{
    public GameObject explodeIndicator;
    public float explosionRadius = .35f;
    public float range = 4.5f;
    int layerMask = ~((1 << 3) | (1 << 8) | (1 << 9) | (1 << 11) | (1 << 13) | (1 << 10));
    Vector2 interactDirection;

    public Animation anim;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        maxMag = 1;
        maxReserve = 0;
        currentMag = 1;
        currentReserve = 0;
        baseDamage = 25;
        fireRate = .5f;
    }
    void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Mouse0) && pauseManager.paused == false && currentMag > 0 && fireCooldown == 0)
        {
            if (canShoot)
                Shoot();
        }
        if (Input.GetKeyDown(KeyCode.R) && pauseManager.paused == false && currentMag != maxMag)
            Reload();

        if (fireCooldown > 0)
            fireCooldown -= Time.deltaTime;
        else if (fireCooldown < 0)
            fireCooldown = 0;

        interactDirection = playerCam.ScreenToWorldPoint(Input.mousePosition) - playerObj.transform.position;
        RaycastHit Crosshairhit;
        if (Physics.Raycast(transform.position, interactDirection, out Crosshairhit, range, layerMask)) //shoot ray from barrel of gun
        {
            Debug.DrawLine(transform.position, Crosshairhit.point);
            Debug.Log((transform.position - Crosshairhit.point).magnitude);
            crosshair.transform.position = Crosshairhit.point;
        }
        else
        {
            crosshair.transform.localPosition = new Vector3(range, 0, 0);
        }
    }

    public void Shoot()
    {
        anim.Stop();
        anim.Play();
        playerMovementScript.setShotRecentlyTimer();
        shootSound.Play();
        fireCooldown = fireRate;
        interactDirection = playerCam.ScreenToWorldPoint(Input.mousePosition) - playerObj.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, interactDirection, out hit, range, layerMask)) //shoot ray from barrel of gun
        {
            Debug.DrawRay(transform.position, interactDirection);
            Debug.Log(hit.collider.gameObject.name.ToString());

            if (hit.collider.gameObject.CompareTag("enemy"))
            {
                hit.collider.gameObject.GetComponent<Enemy>().takeDamage(baseDamage);
                GameObject newExplosiveIndicator = Instantiate(explodeIndicator, hit.point, Quaternion.identity);
                newExplosiveIndicator.GetComponent<ExplosiveRadius>().explosionRadius = .5f;
                foreach (StickyGrenadeProjectile stickyGrenade in hit.collider.gameObject.GetComponentsInChildren<StickyGrenadeProjectile>())
                {
                    stickyGrenade.Explode();
                }
            }

            if (hit.collider.gameObject.CompareTag("explodewhenshot"))
            {
                hit.collider.gameObject.GetComponentInParent<StickyGrenadeProjectile>().Explode();
            }
            if (hit.collider.gameObject.CompareTag("shootplate"))
            {
                hit.collider.gameObject.GetComponent<ShootPlate>().togglePlate();
            }
        }
        //GameObject clone = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
        UpdateHUDValues();
    }

}
