using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    Vector2 interactDirection;
    Vector2 pointerPos;
    public PauseMenu pauseManager;
    public Camera playerCam;
    public weaponManager weaponManager;
    public TextMeshProUGUI interactIndicatorText;
    public TextMeshProUGUI feedbackText;
    public GameObject playerModelHand;
    public GameObject playerModelHead;

    int usefulItemsInCollider = 0; //this is so the indicator text does not reset to " " if there is an item in collider, but none in raycast LOS

    int layerMask = ~((1 << 9) | (1 << 13) | (1 << 3));

    // Start is called before the first frame update
    void Start()
    {
        pauseManager = FindObjectOfType<PauseMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit2;
        if (Physics.Raycast(playerModelHead.transform.position, interactDirection, out hit2, 6, layerMask) && pauseManager.paused == false) //shoot ray in front of player, check every frame if item is interactable
        {
            if (hit2.collider.gameObject.CompareTag("droppedweapon"))
            {
                interactIndicatorText.text = "Click 'E' to pick up weapon";
            }
            else if (hit2.collider.gameObject.CompareTag("lever"))
            {
                interactIndicatorText.text = "Click 'E' to toggle the lever";
            }
            else if (usefulItemsInCollider == 0)
            {
                interactIndicatorText.text = " ";
            }
        }
        else if (usefulItemsInCollider == 0) //no weapon in front of player
        {
            interactIndicatorText.text = " ";
        }
            interactDirection = playerCam.ScreenToWorldPoint(Input.mousePosition) - playerModelHead.transform.position;
        if (Input.GetKeyDown(KeyCode.E) && pauseManager.paused == false)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerModelHead.transform.position, interactDirection, out hit, 6, layerMask)) //shoot ray in front of player
            {
                Debug.DrawRay(playerModelHead.transform.position, interactDirection * 2);
                Debug.Log(hit.ToString());

                if (hit.collider.gameObject.CompareTag("lever"))
                    hit.collider.gameObject.GetComponentInParent<LeverScript>().triggerDoorMaster();
                else if (hit.collider.gameObject.CompareTag("droppedweapon"))
                {
                    if (weaponManager.weaponInventory.Count < weaponManager.maxWeapons) //does player have inventory space to pick up new weapon
                    {
                        hit.collider.gameObject.transform.SetParent(playerModelHand.transform, false);
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
                    else if (weaponManager.weaponInventory.Count >= weaponManager.maxWeapons) //if inventory is full
                    {
                        feedbackText.text = "Inventory is full. Drop with G";
                        feedbackText.color = new Color(feedbackText.color.r,feedbackText.color.g,feedbackText.color.b, 1);
                        Invoke("startFadingText", 1);
                    }
                }
            }
        }
    }

    void startFadingText()
    {
        StartCoroutine(fadeFeedbackText(3));
    }
    public IEnumerator fadeFeedbackText(float time)
    {
        feedbackText.color = new Color(feedbackText.color.r, feedbackText.color.g, feedbackText.color.b, 1);
        while(feedbackText.color.a > 0.0f)
        {
            feedbackText.color = new Color(feedbackText.color.r, feedbackText.color.g, feedbackText.color.b, feedbackText.color.a - (Time.deltaTime / time));
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<LeverScript>() != null)
        {
            usefulItemsInCollider++;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<LeverScript>() != null)
        {
            interactIndicatorText.text = "Click 'E' to toggle the lever";
            if (Input.GetKeyDown(KeyCode.E))
            {
                other.gameObject.GetComponent<LeverScript>().triggerDoorMaster();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<LeverScript>() != null)
        {
            usefulItemsInCollider--;
        }
    }
}
