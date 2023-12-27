using UnityEngine;

public class ShootPlate : MonoBehaviour
{
    public AudioSource activateSound;
    public AudioSource deactivateSound;
    DoorMaster doorMaster;
    Transform defaultTransform;
    Transform compressedTransform;
    public bool interactable = true;
    // Start is called before the first frame update
    void Start()
    {
        doorMaster = GetComponentInParent<DoorMaster>();
        defaultTransform = transform;
        compressedTransform = transform;
        compressedTransform.localScale = new Vector3(defaultTransform.localScale.x, defaultTransform.localScale.y/2, defaultTransform.localScale.z);
        Debug.Log(defaultTransform.localScale);
    }


    public void togglePlate()
    {
        if (interactable)
        {
            doorMaster.openDoor();
            activatePlate();
            Invoke("deactivatePlate", doorMaster.openTime);
        }
    }

    public void activatePlate()
    {
        transform.localScale = compressedTransform.localScale;
        setUninteractable();
        activateSound.Play();
    }

    public void deactivatePlate()
    {
        transform.localScale = defaultTransform.localScale;
        Debug.Log(defaultTransform.localScale);
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