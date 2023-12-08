using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    public GameObject activeLever;
    public GameObject inactiveLever;
    public GameObject doorMaster;
    AudioSource leverSound;

    bool active = false;

    private void Start()
    {
        leverSound = GetComponent<AudioSource>();
    }
    public void triggerDoorMaster()
    {
        doorMaster.GetComponent<DoorMaster>().toggleDoorLever(active);
    }

    public void activateLever()
    {
        activeLever.SetActive(true);
        inactiveLever.SetActive(false);
        active = true;
        leverSound.Play();
    }
    public void deactivateLever()
    {
        inactiveLever.SetActive(true);
        activeLever.SetActive(false);
        active = false;
        leverSound.Play();
    }
}
