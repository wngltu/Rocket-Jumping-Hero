using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public BoxCollider col;

    private void Start()
    {
        col.enabled = false;
    }

    public void enablePlayerCollisions() //call when player is about to land on platform
    {
        col.enabled = true;
        print("enablecol");
    }
    
    public void disablePlayerCollisions() //call when player clicks S while standing on platform, or when player jumps/falls off platform
    {
        col.enabled = false;
        print("disablecol");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
             if (other.GetComponent<CharacterController>().velocity.y < 0) //is the player falling from above the platform?
            {
                enablePlayerCollisions();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) //if player jumps or walks off platform, disable collisions
        {
            disablePlayerCollisions();
        }
    }


}
