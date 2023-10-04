using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolPickup : MonoBehaviour
{
    public weaponManager weaponManagerScript;
    public weaponScript pistol;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PickUp()
    {
        weaponManagerScript.addWeapon(pistol);
    }
    public void Delete()
    {
        Destroy(gameObject);
    }
}
