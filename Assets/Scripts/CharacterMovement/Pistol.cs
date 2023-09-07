using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : weaponScript
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("AddWeapon", .5f);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
