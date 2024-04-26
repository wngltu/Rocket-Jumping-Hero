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
    public PlayerRocketLauncher rocket;
    public GameObject reloadCircle;
    public GameObject mainCrosshair;
    public GameObject reloadCircleBackground;
    public Slider slider;
    float filltime;
    // Start is called before the first frame update
    void Start()
    {
        wep = FindObjectOfType<weaponManager>();
        rocket = FindObjectOfType<PlayerRocketLauncher>();
    }

    // Update is called once per frame
    void Update()
    {
        crosshair.transform.position = Input.mousePosition;

        if (wep.weaponInventory.Count != 0)
        {
            if (wep.equippedWeapon.isReloading == true)
            {
                slider.value += wep.equippedWeapon.reloadTime * Time.deltaTime;
            }
            else
            {
                slider.value = wep.equippedWeapon.currentMag / wep.equippedWeapon.maxMag;
            }

            if (rocket.equipped == false)
            {
                mainCrosshair.GetComponent<Image>().color = Color.red;
            }
            else if (rocket.equipped == true)
            {
                mainCrosshair.GetComponent<Image>().color = Color.blue;
            }
        }
        else
        {
            slider.value = 0;
            if (rocket.equipped == false)
            {
                mainCrosshair.GetComponent<Image>().color = new Color(1,0,0,.5f);
            }
            else if (rocket.equipped == true)
            {
                mainCrosshair.GetComponent<Image>().color = Color.blue;
            }
        }
    }

    public void Reload()
    {
        slider.value = 0;
    }

}
