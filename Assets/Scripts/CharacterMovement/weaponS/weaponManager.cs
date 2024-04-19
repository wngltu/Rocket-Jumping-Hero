using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class weaponManager : MonoBehaviour
{
    public static weaponManager Instance;
    public List<weaponScript> weaponInventory = new List<weaponScript>();
    public int equippedNum = 0;
    public int maxWeapons = 3;
    public weaponScript equippedWeapon;
    public PauseMenu pauseManager;
    public TextMeshProUGUI magText;
    public TextMeshProUGUI reserveText;
    public GameObject playerItemDrop;
    public PlayerRocketLauncher playerRocketLauncherScript;
    public AudioSource playerDropSFX;
    InventoryUIScript playerInvScript;
    public AudioSource reloadSFX;
    public AudioSource reloadFinishSFX;
    public playerAnimationScript animScript;

    public bool isReloading;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        pauseManager = FindObjectOfType<PauseMenu>();
        playerInvScript = FindObjectOfType<InventoryUIScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && weaponInventory.Count > 1 && pauseManager.paused == false && !Input.GetKey(KeyCode.LeftControl))
        {
            if (equippedNum > 0)
            {
                equippedNum--;
            }
            else
            {
                equippedNum = weaponInventory.Count-1;
            }
            equippedWeapon = weaponInventory[equippedNum];
            EquipCurrentWeapon();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && weaponInventory.Count > 1 && pauseManager.paused == false && !Input.GetKey(KeyCode.LeftControl))
        {
            if (equippedNum < weaponInventory.Count-1)
            {
                equippedNum++;
            }
            else
            {
                equippedNum = 0; 
            }
            equippedWeapon = weaponInventory[equippedNum];
            EquipCurrentWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) && pauseManager.paused == false && weaponInventory.Count > 0)
        {
            equippedNum = 0;
            EquipCurrentWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && pauseManager.paused == false && weaponInventory.Count > 1)
        {
            equippedNum = 1;
            EquipCurrentWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && pauseManager.paused == false && weaponInventory.Count > 2) 
        {
            equippedNum = 2;
            EquipCurrentWeapon();
        }

        if ((Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.F)) && pauseManager.paused == false) //drop weapon
        {
            DropCurrentWeapon();
        }

        if (weaponInventory.Count > 0)
            isReloading = equippedWeapon.isReloading;
        else
            isReloading = false;    
    }

    public void UnequipAll()
    {
        foreach (weaponScript weapon in weaponInventory)
        {
            weapon.Unequip();
            weapon.enabled = false;
        }
    }
    public void EquipCurrentWeapon()
    {
        if (weaponInventory.Count > 0)
        {
            animScript.anim.SetTrigger("isSwitchingWeapons");
            equippedWeapon = weaponInventory[equippedNum];
            UnequipAll();
            equippedWeapon.enabled = true;
            equippedWeapon.Equip();
            playerInvScript.switchWeapons(equippedNum);
        }
    }

    public void addWeapon(weaponScript weapon)
    {
        weaponInventory.Add(weapon);
        weapon.lootPillar.SetActive(false);
        playerInvScript.updateWeaponInventory();
    }

    public void DropWeapon(weaponScript weapon)
    {
        if (weaponInventory.Count > 0)
        {
            playerDropSFX.Play();
            int temp = equippedNum;
            weaponInventory[equippedNum].GetComponent<Rigidbody>().isKinematic = false;
            weaponInventory[equippedNum].transform.SetParent(null);
            //weapon.transform.position = weapon.barrel.transform.position;
            if (weapon.crosshair != null)
                weapon.crosshair.SetActive(false);
            weaponInventory[equippedNum].GetComponent<weaponScript>().enabled = false;
            weapon.lootPillar.SetActive(true);
            weapon.GetComponent<Collider>().enabled = true;
            if (weapon.reloadInterrupted == false)
                weapon.reloadInterrupted = true;
            weaponInventory.Remove(weapon);
            magText.text = " ";
            reserveText.text = " ";
            playerInvScript.updateWeaponInventory();
            /*
            if (temp >= 0 && weaponInventory.Count > 0)
            {
                equippedNum = temp;
                EquipCurrentWeapon();
            }
            else if (weaponInventory.Count != 0)
            {
                equippedNum = 1;
                EquipCurrentWeapon();
            }*/
        }
    }
    public void DropEmptyWeapon(weaponScript weapon)
    {
        if (weaponInventory.Count > 0)
        {
            playerDropSFX.Play();
            int temp = equippedNum;
            weaponInventory[equippedNum].GetComponent<Rigidbody>().isKinematic = false;
            weaponInventory[equippedNum].transform.SetParent(null);
            weaponInventory[equippedNum].gameObject.GetComponent<Rigidbody>().AddForce(Random.Range(-100f, 100f), Random.Range(200f, 200f), 0f);
            weaponInventory[equippedNum].gameObject.GetComponent<Rigidbody>().AddTorque(0, 0, 200);
            weaponInventory[equippedNum].Invoke("DeleteWeapon", 3);
            //weapon.transform.position = weapon.barrel.transform.position;
            if (weapon.crosshair != null)
                weapon.crosshair.SetActive(false);
            weaponInventory[equippedNum].GetComponent<weaponScript>().enabled = false;
            weapon.GetComponent<Collider>().enabled = false;
            if (weapon.reloadInterrupted == false)
                weapon.reloadInterrupted = true;
            weaponInventory.Remove(weapon);
            magText.text = " ";
            reserveText.text = " ";
            playerInvScript.updateWeaponInventory();
        }
    }

    public void DropCurrentWeapon()
    {
        if (playerRocketLauncherScript.equipped == false)
        {
            if (equippedNum > 0 && weaponInventory.Count > 1)
            {
                DropWeapon(equippedWeapon);
                equippedNum--;
                EquipCurrentWeapon();
            }
            else if (equippedNum > 0 && weaponInventory.Count == 1)
            {
                DropWeapon(equippedWeapon);
                equippedNum = 0;
            }
            else if (equippedNum == 0 && weaponInventory.Count <= 3) //if this is the last weapon in inventory dont try to switch
            {
                DropWeapon(equippedWeapon);
                equippedNum = 0;
                EquipCurrentWeapon();
            }
        }
    }

    public void DropCurrentEmptyWeapon()
    {
        if (playerRocketLauncherScript.equipped == false)
        {
            if (equippedNum > 0 && weaponInventory.Count > 1)
            {
                DropEmptyWeapon(equippedWeapon);
                equippedNum--;
                EquipCurrentWeapon();
            }
            else if (equippedNum > 0 && weaponInventory.Count == 1)
            {
                DropEmptyWeapon(equippedWeapon);
                equippedNum = 0;
            }
            else if (equippedNum == 0 && weaponInventory.Count <= 3) //if this is the last weapon in inventory dont try to switch
            {
                DropEmptyWeapon(equippedWeapon);
                equippedNum = 0;
                EquipCurrentWeapon();
            }
        }
    }

}
