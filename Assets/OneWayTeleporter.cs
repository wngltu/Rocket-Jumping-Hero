using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayTeleporter : MonoBehaviour
{
    public GameObject teleportDestination;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("teleportertest2");
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("teleporter");
            other.gameObject.GetComponent<CharacterController>().enabled = false;
            other.gameObject.transform.position = teleportDestination.transform.position;
            other.gameObject.GetComponent<CharacterController>().enabled = true;
        }
    }
}
