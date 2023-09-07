using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : weaponScript
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Unequip", .75f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
