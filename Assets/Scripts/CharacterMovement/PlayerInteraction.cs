using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    Vector2 interactDirection;
    Vector2 pointerPos;
    public Camera playerCam;
    public weaponManager weaponManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        interactDirection = playerCam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, interactDirection, out hit, 6)) //shoot ray in front of player
            {
                Debug.DrawRay(transform.position, interactDirection * 2);
                Debug.Log(hit.ToString());

                if (hit.collider.gameObject.CompareTag("lever"))
                    hit.collider.gameObject.GetComponentInParent<LeverScript>().triggerDoorMaster();
                else if (hit.collider.gameObject.CompareTag("droppedweapon"))
                {
                    hit.collider.gameObject.transform.SetParent(this.transform, false);
                    hit.collider.gameObject.transform.localPosition = new Vector3(0, 0, 0);
                    hit.collider.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    hit.collider.gameObject.GetComponent<weaponScript>().Unequip();
                    hit.collider.gameObject.GetComponent<weaponScript>().enabled = false;
                    hit.collider.enabled = false;
                    weaponManager.addWeapon(hit.collider.gameObject.GetComponent<weaponScript>());
                    if (weaponManager.weaponInventory.Count == 1) //is this the first weapon the player picks up? if so equip it
                    {
                        weaponManager.equippedNum = 0;
                        weaponManager.EquipCurrentWeapon();
                    }
                }
            }
        }
    }
}
