using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Grenade : weaponScript
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
        maxReserve = 2;
        currentMag = maxMag;
        currentReserve = maxReserve;
        baseDamage = 25;
        fireRate = 1f;
    }
    void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Mouse0) && pauseManager.paused == false && currentMag > 0 && fireCooldown == 0)
        {
            if (canShoot)
                Shoot();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && pauseManager.paused == false && currentReserve == 0 && currentMag == 0)
        {
            weaponManager.DropCurrentEmptyWeapon();
        }
        /*else if (Input.GetKeyDown(KeyCode.Mouse0) && pauseManager.paused == false && currentMag <= 0)
        {
            if (canReload)
            {
                Reload();
                Invoke("ShowModel", reloadTime);
            }
        }*/
        if (Input.GetKeyDown(KeyCode.R) && pauseManager.paused == false && currentMag != maxMag)
        {
            Reload();
            Invoke("ShowModel", reloadTime);
        }

        if (fireCooldown > 0)
            fireCooldown -= Time.deltaTime;
        else if (fireCooldown < 0)
            fireCooldown = 0;
    }

    public void Shoot()
    {
        playerMovementScript.setShotRecentlyTimer();
        fireCooldown = fireRate;
        currentMag--;
        if (currentMag <= 0)
            model.gameObject.SetActive(false);
        UpdateHUDValues();
        interactDirection = playerCam.ScreenToWorldPoint(Input.mousePosition) - playerObj.transform.position;
        GameObject newobject = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
        print(barrel.transform.position.ToString());
        newobject.transform.SetParent(null);
        if (currentReserve > 0)
        {
            Invoke("ShowModel", reloadTime);
        }
        Reload();
    }
    public void ShowModel()
    {
        model.gameObject.SetActive(true);
    }
}
