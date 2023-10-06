using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    public weaponScript weaponScript;
    public Rigidbody rb;
    public BoxCollider col;
    public Transform player, gunContainer;

    public float dropForce;

    public bool equipped;
    public static bool slotFull;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            Drop();
    }
    public void Pickup()
    {
        equipped = true;
        slotFull = true;

        rb.isKinematic = true;
        col.isTrigger = true;

        weaponScript.enabled = true;
    }
    public void Drop()
    {
        equipped = false;
        slotFull = false;

        rb.isKinematic = false;
        col.isTrigger = false;

        weaponScript.enabled = false;
    }
}
