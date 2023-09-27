using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    Vector2 interactDirection;
    Vector2 pointerPos;
    public Camera playerCam;
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
            if (Physics.Raycast(transform.position, interactDirection, out hit, 4)) //shoot ray in front of player
            {
                Debug.DrawRay(transform.position, interactDirection * 2);
                Debug.Log(hit.ToString());

                if (hit.collider.gameObject.CompareTag("lever"))
                {
                    hit.collider.gameObject.GetComponentInParent<LeverScript>().triggerDoorMaster();
                }
            }
        }
    }
}
