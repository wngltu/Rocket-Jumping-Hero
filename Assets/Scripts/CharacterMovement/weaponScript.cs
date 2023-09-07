using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponScript : MonoBehaviour
{
    public static weaponScript Instance;
    public Camera playerCam;
    public GameObject bullet;
    public GameObject barrel;

    public bool equipped = false;

    Vector2 pointerPos;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        Instance = this;
        Invoke("AddWeapon", .5f);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject clone = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
        }

        pointerPos = Input.mousePosition;
        
        pointerPos = playerCam.ScreenToWorldPoint(pointerPos);
        pointerPos = pointerPos - (Vector2)transform.position;
        transform.right = pointerPos;
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
    public virtual void AddWeapon()
    {
        weaponManager.Instance.weaponInventory.Add(this);
    }
}
