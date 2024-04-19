using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class crosshairScript : MonoBehaviour
{
    public Camera playerCam;
    public GameObject crosshair;
    public weaponManager wep;
    public GameObject reloadCircle;
    public GameObject mainCrosshair;
    public Slider slider;
    float filltime;
    // Start is called before the first frame update
    void Start()
    {
        wep = FindObjectOfType<weaponManager>();
    }

    // Update is called once per frame
    void Update()
    {
        crosshair.transform.position = Input.mousePosition;

        if (wep.weaponInventory.Count != 0)
        {
            if (wep.equippedWeapon.isReloading == true)
            {
                mainCrosshair.GetComponent<Image>().color = Color.black;
                slider.value += wep.equippedWeapon.reloadTime * Time.deltaTime;
            }
            else
            {
                slider.value = wep.equippedWeapon.currentMag / wep.equippedWeapon.maxMag;
                mainCrosshair.GetComponent<Image>().color = Color.white;
            }
        }
        else
        {
            mainCrosshair.GetComponent<Image>().color = Color.white;
            slider.value = 0;
        }
    }

    public void Reload()
    {
        slider.value = 0;
    }

}
