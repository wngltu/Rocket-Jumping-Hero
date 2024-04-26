using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public AudioSource activateSound;
    public AudioSource deactivateSound;
    DoorMaster doorMaster;
    public bool interactable = true;
    // Start is called before the first frame update
    void Start()
    {
        doorMaster = GetComponentInParent<DoorMaster>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            doorMaster.openDoor();
            activatePlate();
            Invoke("deactivatePlate", doorMaster.openTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            doorMaster.openDoor();
            activatePlate();
            Invoke("deactivatePlate", doorMaster.openTime);
        }
    }

    public void activatePlate()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / 2, transform.localScale.z);
        setUninteractable();
        activateSound.Play();
    }

    public void deactivatePlate()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 2, transform.localScale.z);
        deactivateSound.Play();
    }   

    void setUninteractable()
    {
        interactable = false;
        Invoke("setInteractable", doorMaster.openTime);
    }
    void setInteractable()
    {
        interactable = true;
        deactivatePlate();
    }
}
