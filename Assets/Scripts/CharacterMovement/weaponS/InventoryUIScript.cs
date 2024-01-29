using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUIScript : MonoBehaviour
{
    weaponManager weaponManagerScript;
    public GameObject slot1UI, slot2UI, slot3UI;
    public TextMeshProUGUI slot1WepName, slot2WepName, slot3WepName;
    //public TextMeshProUGUI slot1CurrentAmmo, slot2CurrentAmmo, slot3CurrentAmmo;
    //public TextMeshProUGUI slot1ReserveAmmo, slot2ReserveAmmo, slot3ReserveAmmo;

    private void Start()
    {
        weaponManagerScript = FindObjectOfType<weaponManager>();
    }

    public void switchWeapons(int currentWep) //equippedweapon from wepmanage should be passed through
    {
        switch (currentWep)
        {
            case 0:
                setUIColor(slot1UI, Color.white);
                setUIColor(slot2UI, Color.gray);
                setUIColor(slot3UI, Color.gray);
                break;
            case 1:
                setUIColor(slot1UI, Color.gray);
                setUIColor(slot2UI, Color.white);
                setUIColor(slot3UI, Color.gray);
                break;
            case 2:
                setUIColor(slot1UI, Color.gray);
                setUIColor(slot2UI, Color.gray);
                setUIColor(slot3UI, Color.white);
                break;
        }
    }

    public void addWeapon(int currentWep, weaponScript weapon)
    {
        switch (currentWep)
        {
            case 0:
                slot1UI.SetActive(true);
                slot1WepName.text = weapon.name;
                break;
            case 1:
                slot2UI.SetActive(true);
                slot2WepName.text = weapon.name;
                break;
            case 2:
                slot3UI.SetActive(true);
                slot3WepName.text = weapon.name;
                break;
        }
    }

    public void dropWeapon(int currentWep)
    {
        switch (currentWep)
        {
            case 0:
                slot1UI.SetActive(false);
                break;
            case 1:
                slot2UI.SetActive(false);
                break;
            case 2:
                slot3UI.SetActive(false);
                break;
        }
    }
        
    public void setUIColor(GameObject slotUI, Color color)
    {
        foreach (TextMeshProUGUI text in slotUI.GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.GetComponent<TextMeshProUGUI>().color = color;
        }
    }

    public void updateWeaponInventory()
    {
        if (weaponManagerScript.weaponInventory.Count > 0)
            slot1WepName.text = weaponManagerScript.weaponInventory[0].name;
        else
            slot1WepName.text = "";
        if (weaponManagerScript.weaponInventory.Count > 1)
            slot2WepName.text = weaponManagerScript.weaponInventory[1].name;
        else
            slot2WepName.text = "";
        if (weaponManagerScript.weaponInventory.Count > 2)
            slot3WepName.text = weaponManagerScript.weaponInventory[2].name;
        else
            slot3WepName.text = "";
    }
}
