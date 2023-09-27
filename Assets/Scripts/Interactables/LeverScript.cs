using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    public GameObject activeLever;
    public GameObject inactiveLever;
    public GameObject doorMaster;

    bool active = false;
    public void triggerDoorMaster()
    {
        doorMaster.GetComponent<DoorMaster>().toggleDoorLever(active);
    }

    public void activateLever()
    {
        activeLever.SetActive(true);
        inactiveLever.SetActive(false);
        active = true;
    }
    public void deactivateLever()
    {
        inactiveLever.SetActive(true);
        activeLever.SetActive(false);
        active = false;
    }
}
