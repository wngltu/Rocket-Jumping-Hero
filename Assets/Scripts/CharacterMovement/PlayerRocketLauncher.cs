using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRocketLauncher : MonoBehaviour
{
    public PlayerRocketLauncher Instance;
    public Camera playerCam;
    public GameObject bullet;
    public GameObject barrel;
    public GameObject model;
    public PlayerMovement playerMovement;
    public PauseMenu pauseManager;

    public bool equipped = false;

    Vector2 pointerPos;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        Instance = this;
        playerMovement = FindObjectOfType<PlayerMovement>();
        pauseManager = FindObjectOfType<PauseMenu>();

        model.gameObject.SetActive(false);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!pauseManager.paused)
        {
            pointerPos = Input.mousePosition;
            pointerPos = playerCam.ScreenToWorldPoint(pointerPos);
            pointerPos = pointerPos - (Vector2)transform.position;
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
        model.gameObject.SetActive(true);
        playerMovement.setShotRecentlyTimer();
        equipped = true;
    }
    public virtual void Unequip()
    {
        model.gameObject.SetActive(false);
        equipped = false;
    }

    public void Shoot()
    {
        if (Input.mousePosition.x < Screen.width / 2f) //if mouse on left half of screen
        {
            GameObject clone = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation * new Quaternion(0,0,-1,0));
        }
        else //if mouse on right half of screen
        {
            GameObject clone = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
        }
        playerMovement.rocketFireCooldownTimer = playerMovement.rocketFireRate;
        playerMovement.rocketRecentlyShotTimer = playerMovement.rocketRecentlyShotTime;
    }
}
