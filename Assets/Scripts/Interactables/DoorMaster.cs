using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorMaster : MonoBehaviour
{
    public List<GameObject> doors;
    public List<GameObject> pressurePlates;
    public List<GameObject> levers;
    public List<GameObject> reverseDoors;
    public float openTime = 5f;
    public float doorTimer;

    public float toggleCooldown = .1f;
    public float toggleCooldownTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in transform)
        {
            if (child.CompareTag("door")) //object is a door
                doors.Add(child.gameObject);
            else if (child.CompareTag("pressureplate")) //object is a pressure plate
                pressurePlates.Add(child.gameObject);
            else if (child.CompareTag("lever")) //object is a lever
                levers.Add(child.gameObject);
            else if (child.CompareTag("rdoor")) //object is a reverse door (for lever)
                reverseDoors.Add(child.gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (toggleCooldownTimer > 0)
            toggleCooldownTimer -= Time.deltaTime;
        else if (toggleCooldownTimer < 0)
            toggleCooldownTimer = 0;
    }

    public void openDoor()
    {
        toggleCooldownTimer = toggleCooldown;
        foreach (GameObject door in doors)
        {
            door.SetActive(false);
        }
        foreach(GameObject plate in pressurePlates)
        {
            if (plate.GetComponent<PressurePlate>().interactable == true)
            {
                plate.GetComponent<PressurePlate>().activatePlate();
            }
        }
        Invoke("closeDoor", openTime);
    }
    public void closeDoor()
    {
        foreach (GameObject door in doors)
        {
            door.SetActive(true);
        }    
    }

    public void toggleDoorLever(bool active)
    {
        if (toggleCooldownTimer == 0)
        {
            toggleCooldownTimer = toggleCooldown;
            if (active)
                closeDoorLever();
            else if (!active)
                openDoorLever();
        }
    }

    public void openDoorLever()
    {
        foreach (GameObject door in doors)
            door.SetActive(false);
        foreach (GameObject lever in levers)
            lever.GetComponent<LeverScript>().activateLever();
        foreach (GameObject rdoor in reverseDoors)
            rdoor.SetActive(true);
    }

    public void closeDoorLever()
    {
        foreach (GameObject door in doors)
            door.SetActive(true);
        foreach (GameObject lever in levers)
            lever.GetComponent<LeverScript>().deactivateLever();
        foreach (GameObject rdoor in reverseDoors)
            rdoor.SetActive(false);
    }
}
