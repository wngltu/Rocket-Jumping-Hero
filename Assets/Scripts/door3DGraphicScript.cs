using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door3DGraphicScript : MonoBehaviour
{
    public GameObject doorGraphic;
    public void disableDoorGraphic()
    {
        doorGraphic.SetActive(false);
    }
    public void enableDoorGraphic()
    {
        doorGraphic.SetActive(true);
    }
}
