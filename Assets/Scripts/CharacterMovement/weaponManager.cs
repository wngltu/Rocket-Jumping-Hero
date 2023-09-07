using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponManager : MonoBehaviour
{
    public static weaponManager Instance;
    public List<weaponScript> weaponInventory = new List<weaponScript>();
    public int equippedNum = 0;
    public weaponScript equippedWeapon;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && weaponInventory.Count > 1)
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
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && weaponInventory.Count > 1)
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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            equippedWeapon = weaponInventory[0];
            EquipCurrentWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            equippedWeapon = weaponInventory[1];
            EquipCurrentWeapon();
        }
    }

    public void UnequipAll()
    {
        foreach (weaponScript weapon in weaponInventory)
        {
            weapon.Unequip();
        }
    }
    public void EquipCurrentWeapon()
    {
        UnequipAll();
        equippedWeapon.Equip();
    }
}
