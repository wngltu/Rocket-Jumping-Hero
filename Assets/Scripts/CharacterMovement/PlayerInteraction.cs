using TMPro;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    Vector2 interactDirection;
    Vector2 pointerPos;
    public PauseMenu pauseManager;
    public Camera playerCam;
    public weaponManager weaponManager;
    public TextMeshProUGUI interactIndicatorText;
    int layerMask = ~((1 << 9) | (1 << 13));

    // Start is called before the first frame update
    void Start()
    {
        pauseManager = FindObjectOfType<PauseMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit2;
        if (Physics.Raycast(transform.position, interactDirection, out hit2, 6, layerMask) && pauseManager.paused == false) //shoot ray in front of player, check every frame if item is interactable
        {
            if (hit2.collider.gameObject.CompareTag("droppedweapon"))
            {
                interactIndicatorText.text = "Click 'E' to pick up weapon";
            }
            else if (hit2.collider.gameObject.CompareTag("lever"))
            {
                interactIndicatorText.text = "Click 'E' to toggle the lever";
            }
            else
            {
                interactIndicatorText.text = " ";
            }
        }
        else //no weapon in front of player
        {
            interactIndicatorText.text = " ";
        }
            interactDirection = playerCam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        if (Input.GetKeyDown(KeyCode.E) && pauseManager.paused == false)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, interactDirection, out hit, 6, layerMask)) //shoot ray in front of player
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
