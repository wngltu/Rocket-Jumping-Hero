using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class weaponManager : MonoBehaviour
{
    public static weaponManager Instance;
    public List<weaponScript> weaponInventory = new List<weaponScript>();
    public int equippedNum = 0;
    public weaponScript equippedWeapon;
    public PauseMenu pauseManager;
    public TextMeshProUGUI magText;
    public TextMeshProUGUI reserveText;
    public GameObject playerItemDrop;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && weaponInventory.Count > 1 && pauseManager.paused == false)
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
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && weaponInventory.Count > 1 && pauseManager.paused == false)
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
        if (Input.GetKeyDown(KeyCode.Alpha1) && pauseManager.paused == false)
        {
            equippedNum = 0;
            EquipCurrentWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && pauseManager.paused == false)
        {
            equippedNum = 1;
            EquipCurrentWeapon();
        }

        if (Input.GetKeyDown(KeyCode.G) && pauseManager.paused == false) //drop weapon
        {
            if (equippedNum > 0)
            {
                DropWeapon(equippedWeapon);
                equippedNum--;
                EquipCurrentWeapon();
            }
            else if (equippedNum == 0 && weaponInventory.Count <= 2) //if this is the last weapon in inventory dont try to switch
            {
                DropWeapon(equippedWeapon);
                equippedNum = 0;
                EquipCurrentWeapon();
            }
        }
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
            equippedWeapon = weaponInventory[equippedNum];
            UnequipAll();
            equippedWeapon.enabled = true;
            equippedWeapon.Equip();
        }
    }

    public void addWeapon(weaponScript weapon)
    {
        weaponInventory.Add(weapon);
    }

    public void DropWeapon(weaponScript weapon)
    {
        weaponInventory[equippedNum].GetComponent<Rigidbody>().isKinematic = false;
        weaponInventory[equippedNum].transform.SetParent(null);
        //weapon.transform.position = weapon.barrel.transform.position;
        weaponInventory[equippedNum].GetComponent<weaponScript>().enabled = false;
        weapon.GetComponent<Collider>().enabled = true;
        if (weapon.reloadInterrupted == false)
            weapon.reloadInterrupted = true;
        weaponInventory.Remove(weapon);
        magText.text = " ";
        reserveText.text = " ";
    }
}
