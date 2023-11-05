using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyGrenade : weaponScript
{
    public GameObject explodeIndicator;
    public float explosionRadius = .35f;
    public float range = 10f;
    Vector2 interactDirection;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        maxMag = 1;
        maxReserve = 8;
        currentMag = maxMag;
        currentReserve = maxReserve;
        baseDamage = 30;
        fireRate = .3f;
        reloadTime = .5f;
    }
    void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Mouse0) && pauseManager.paused == false && currentMag > 0 && fireCooldown == 0)
        {
            if (canShoot)
                Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && pauseManager.paused == false && currentMag <= 0)
        {
            if (canReload)
                Reload();
        }
        if (Input.GetKeyDown(KeyCode.R) && pauseManager.paused == false && currentMag != maxMag)
            Reload();

        if (fireCooldown > 0)
            fireCooldown -= Time.deltaTime;
        else if (fireCooldown < 0)
            fireCooldown = 0;
    }

    public void Shoot()
    {
        fireCooldown = fireRate;
        currentMag--;
        UpdateHUDValues();
        interactDirection = playerCam.ScreenToWorldPoint(Input.mousePosition) - playerCam.transform.position;
        GameObject newobject = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
        newobject.transform.SetParent(null);
        Reload();
    }

}