using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : weaponScript
{
    public TrailRenderer bulletTrail;
    public GameObject explodeIndicator;
    public float explosionRadius = .35f;
    public float range = 8f;
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
        if (Physics.Raycast(transform.position, interactDirection, out hit, range)) //shoot ray from barrel of gun
        {
            Debug.DrawRay(transform.position, interactDirection);
            Debug.Log(hit.collider.gameObject.name.ToString());

            if (hit.collider.gameObject.CompareTag("enemy"))
            {
                hit.collider.gameObject.GetComponent<Enemy>().takeDamage(baseDamage);
            }
        }
        TrailRenderer trail = Instantiate(bulletTrail, barrel.transform.position, Quaternion.identity);

        if (hit.collider != null)
        {
            StartCoroutine(SpawnTrail(trail, hit));
        }
        else
        {
            StartCoroutine(SpawnTrail(trail, new Vector2(playerCam.transform.position.x, playerCam.transform.position.y) + targetPos.normalized*range));
        }
        //GameObject clone = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
        currentMag--;
        UpdateHUDValues();
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = transform.forward;

        //if (addbulletspread)
        //{
        // direction += new vector3(randomrange(-bulletspreadvariance.x, bulletspreadvariance.x),
        //(-bulletspreadvariance.y, bulletspreadvariance.y), 0)
        // direction.normalize();
        //}
        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        trail.transform.position = hit.point;
        GameObject newObject = Instantiate(explodeIndicator, hit.point, Quaternion.identity); //spawn a circle showing blast radius
        newObject.GetComponent<ExplosiveRadius>().explosionRadius = explosionRadius;

        Destroy(trail.gameObject, trail.time);
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 target)
    {
        print(target);
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, target, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        trail.transform.position = target;
      
        Destroy(trail.gameObject, trail.time);
    }

}
