using System.Collections;
using UnityEngine;

public class Sniper : weaponScript
{
    public TrailRenderer bulletTrail;
    public GameObject explodeIndicator;
    public float explosionRadius = .35f;
    float range = 17f;
    Vector2 interactDirection;

    public Animation anim;

    int layerMask = ~((1 << 3) | (1 << 8) | (1 << 9) | (1 << 11) | (1 << 13) | (1 << 10));
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        maxMag = 3;
        maxReserve = 9;
        currentMag = maxMag;
        currentReserve = maxReserve;
        baseDamage = 85f;
        fireRate = 2f;
    }
    void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Mouse0) && pauseManager.paused == false && currentMag > 0 && fireCooldown == 0)
        {
            if (canShoot)
                Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && pauseManager.paused == false && currentMag <= 0 && fireCooldown == 0)
        {
            if (currentReserve == 0)
            {
                weaponManager.DropCurrentEmptyWeapon();
            }
            if (canReload)
                Reload();
        }
        if (Input.GetKeyDown(KeyCode.R) && pauseManager.paused == false && currentMag != maxMag && fireCooldown == 0)
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
        base.MuzzleFlash();
        playerMovementScript.setShotRecentlyTimer();
        shootSound.Play();
        fireCooldown = fireRate;
        currentMag--;
        UpdateHUDValues();
        interactDirection = playerCam.ScreenToWorldPoint(Input.mousePosition) - playerObj.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, interactDirection, out hit, range, layerMask)) //shoot ray from barrel of gun
        {
            Debug.DrawRay(transform.position, interactDirection);
            Debug.Log(hit.collider.gameObject.name.ToString());

            if (hit.collider.gameObject.CompareTag("enemy"))
            {
                hit.collider.gameObject.GetComponent<Enemy>().takeDamage(baseDamage);
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
        TrailRenderer trail = Instantiate(bulletTrail, barrel.transform.position, Quaternion.identity);

        if (hit.collider != null)
        {
            StartCoroutine(SpawnTrail(trail, hit));
        }
        else
        {
            StartCoroutine(SpawnTrail(trail, new Vector2(playerObj.transform.position.x, playerObj.transform.position.y) + targetPos.normalized * range));
        }
        //GameObject clone = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
        UpdateHUDValues();
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
        bulletImpactSound.Play();

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
