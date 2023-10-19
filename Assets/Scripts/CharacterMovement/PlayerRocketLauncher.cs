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

    public bool equipped = false;

    Vector2 pointerPos;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        Instance = this;
        playerMovement = this.gameObject.GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    protected virtual void Update()
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
        //transform.right = (pointerPos - (Vector2)transform.position).normalized; //used for enemy tracking?
    }

    public virtual void Equip()
    {
        this.gameObject.SetActive(true);
    }
    public virtual void Unequip()
    {
        this.gameObject.SetActive(false);
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
    }
}
