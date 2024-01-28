using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUIScript : MonoBehaviour
{
    public GameObject slot1UI, slot2UI, slot3UI;

    public void dimUnequipped(int wepSlot) //equippedweapon from wepmanage should be passed through
    {
        switch (wepSlot)
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
}
