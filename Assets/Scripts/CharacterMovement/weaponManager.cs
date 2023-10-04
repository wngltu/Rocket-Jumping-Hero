using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        equippedWeapon = weaponInventory[equippedNum];
        UnequipAll();
        equippedWeapon.enabled = true;
        equippedWeapon.Equip();
    }

    public void addWeapon(weaponScript weapon)
    {
        weaponInventory.Add(weapon);
    }
}
