using System.Collections;
using UnityEngine;

public class Shotgun : weaponScript
{
    public TrailRenderer bulletTrail;
    public GameObject explodeIndicator;
    public float explosionRadius = .35f;
    public float range = 8f;
    public float bulletDeviationMagnitude = 1f;

    Vector2 interactDirection;
    Vector2 bulletDeviation;

    int layerMask = ~((1 << 3) | (1 << 8) | (1 << 9) | (1 << 11) | (1 << 13));
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        maxMag = 5;
        maxReserve = 10;
        currentMag = maxMag;
        currentReserve = maxReserve;
        baseDamage = 10;
        fireRate = .5f;
    }
    void Update()
    {
        base.Update();
        if (Input.GetKey(KeyCode.Mouse0) && pauseManager.paused == false && currentMag > 0 && fireCooldown == 0)
        {
            if (canShoot)
                Shoot();
        }
        else if (Input.GetKey(KeyCode.Mouse0) && pauseManager.paused == false && currentMag <= 0 && fireCooldown == 0)
        {
            if (canReload)
                Reload();
        }
        if (Input.GetKeyDown(KeyCode.R) && pauseManager.paused == false && currentMag != maxMag && fireCooldown == 0)
            Reload();

        if (fireCooldown > 0)
            fireCooldown -= Time.deltaTime;
        else if (fireCooldown < 0)
            fireCooldown = 0;
    }

    public void Shoot()
    {
        shootSound.Play();
        fireCooldown = fireRate;
        currentMag--;
        UpdateHUDValues();
        for (int i = 0; i < 8; i++)
        {
            interactDirection = playerCam.ScreenToWorldPoint(Input.mousePosition) - playerCam.transform.position;
            RaycastHit hit;
            bulletDeviation = new Vector2(Random.Range(-bulletDeviationMagnitude, bulletDeviationMagnitude), Random.Range(-bulletDeviationMagnitude, bulletDeviationMagnitude));
            if (Physics.Raycast(transform.position, interactDirection + bulletDeviation, out hit, range, layerMask)) //shoot ray from barrel of gun
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

            if (hit.collider != null) // if bullet hit something, draw trail
            {
                StartCoroutine(SpawnTrail(trail, hit, Random.Range(0, .05f)));
            }
            else //if bullet missed
            {
                StartCoroutine(SpawnTrail(trail, new Vector2(playerCam.transform.position.x, playerCam.transform.position.y) + (targetPos.normalized * range) + bulletDeviation, Random.Range(0, .1f)));
            }
        }
    }
    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit, float delay)
    {
        yield return new WaitForSeconds(delay);

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

    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 target, float delay)
    {
        yield return new WaitForSeconds(delay);
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
