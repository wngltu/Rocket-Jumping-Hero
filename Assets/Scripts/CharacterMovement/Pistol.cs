using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : weaponScript
{
    Vector2 interactDirection;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        maxMag = 7;
        maxReserve = 21;
        currentMag = maxMag;
        currentReserve = maxReserve;
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
        if (Physics.Raycast(transform.position, interactDirection, out hit, 7)) //shoot ray from barrel of gun
        {
            Debug.DrawRay(transform.position, interactDirection * 2);
            Debug.Log(hit.collider.gameObject.name.ToString());

            if (hit.collider.gameObject.CompareTag("enemy"))
            {
                hit.collider.gameObject.GetComponent<Enemy>().takeDamage(baseDamage);
            }
        }
        //GameObject clone = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
        currentMag--;
        UpdateHUDValues();
    }

}
