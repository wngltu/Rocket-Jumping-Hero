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
    public GameObject explosionObject;
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
        playerMovementScript.setShotRecentlyTimer();
        interactDirection = playerCam.ScreenToWorldPoint(Input.mousePosition) - playerObj.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, interactDirection, out hit, range, layerMask)) //shoot ray from barrel of gun
        {
            if (hit.collider != null) //if did not miss
            {
                GameObject newObj = Instantiate(explosionObject, transform.position, Quaternion.identity);
                newObj.GetComponent<ExplosionScript>().PlayerExplode(baseDamage, explosionRadius, explosionForce);
                currentMag--;
                grenadeVisual.SetActive(false);
            }
        }
        UpdateHUDValues();
    }
}
