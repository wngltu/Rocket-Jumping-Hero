using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class weaponScript : MonoBehaviour
{
    public static weaponScript Instance;
    public Camera playerCam;
    public GameObject bullet;
    public GameObject barrel;
    public GameObject model;
    public PauseMenu pauseManager;
    public weaponManager weaponManager;
    public AudioSource shootSound;
    public AudioSource bulletImpactSound;

    public bool equipped = false;
    public bool canShoot = true;
    public bool canReload = true;
    public bool isReloading = false;
    public bool reloadInterrupted = false;

    protected float currentMag;
    protected float maxMag;
    protected float currentReserve;
    protected float maxReserve;
    protected float baseDamage;
    protected float reloadTime = 1;
    protected float fireRate;
    protected float fireCooldown;


    public Vector2 pointerPos;
    public Vector2 targetPos;
    // Start is called before the first frame update
    protected void Start()
    {
        playerCam = FindObjectOfType<Camera>();
        pauseManager = FindObjectOfType<PauseMenu>();
        weaponManager = FindObjectOfType<weaponManager>();
        this.enabled = false;
    }
    // Update is called once per frame
    protected void Update()
    {
        if (!pauseManager.paused)
        {
            pointerPos = Input.mousePosition;
            pointerPos = playerCam.ScreenToWorldPoint(pointerPos);
            pointerPos = pointerPos - (Vector2)transform.position;
            targetPos = pointerPos;
            transform.right = pointerPos;

            if (Input.mousePosition.x < Screen.width / 2f) //if mouse on left half of screen
            {
                model.transform.localScale = new Vector3(1, -1, 1);
            }
            else //if mouse on right half of screen
            {
                model.transform.localScale = Vector3.one;
            }
        }
    }

    public virtual void Equip()
    {
        this.gameObject.SetActive(true);
        UpdateHUDValues();
    }
    public virtual void Unequip()
    {
        if (reloadInterrupted == false && isReloading == true)
            reloadInterrupted = true;
        canShoot = true;
        isReloading = false;
        this.gameObject.SetActive(false);
    }
    public virtual void AddWeapon()
    {
        weaponManager.Instance.weaponInventory.Add(Instance);
        this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    public virtual void Reload()
    {
        if (isReloading == false && canShoot)
        {
            if (currentReserve >= maxMag) //if more ammo than max mag
            {
                reloadInterrupted = false;
                canShoot = false;
                isReloading = true;
                weaponManager.magText.text = "...";
                Invoke("FinishReload", reloadTime);
            }
            else if (currentReserve < maxMag) //if less ammo than max mag, check to see if it can fill to full
            {
                if (currentReserve > 0)
                {
                    canShoot = false;
                    isReloading = true;
                    weaponManager.magText.text = "...";
                    Invoke("FinishReload", reloadTime);
                    reloadInterrupted = false;
                }
                else //reload fail, no reserve ammo
                {
                }
            }
        }
    }
    public virtual void FinishReload()
    {
        if (!reloadInterrupted)
        {
            if (currentReserve >= maxMag) //if more ammo than max mag
            {
                currentReserve -= maxMag - currentMag;
                currentMag = maxMag;
                canShoot = true;
                isReloading = false;
            }
            else if (currentReserve < maxMag) //if less ammo than max mag, check to see if it can fill to full
            {
                if (currentReserve > 0)
                {
                    if ((currentReserve + currentMag) >= maxMag) //if player have more reserve to max out mag
                    {
                        currentReserve -= maxMag - currentMag;
                        currentMag = maxMag;
                    }
                    else if ((currentReserve + currentMag) < maxMag) //if player does not have enough 
                    {
                        currentMag = currentMag + currentReserve;
                        currentReserve = 0;
                    }
                }
                weaponManager.magText.text = currentMag.ToString();
                canShoot = true;
                isReloading = false;
            }
            UpdateHUDValues();
        }
        else //reload was interrupted, so do nothing (except reset reloadInterrupted)
        {
            reloadInterrupted = false;
            canShoot = true;
            isReloading = false;
        }
    }

    public virtual void UpdateHUDValues()
    {
        if (isReloading == true)
        {
            weaponManager.magText.text = "...";
            weaponManager.reserveText.text = currentReserve.ToString();
        }
        else if (isReloading == false)
        {
            weaponManager.magText.text = currentMag.ToString();
            weaponManager.reserveText.text = currentReserve.ToString();
        }
    }
}
