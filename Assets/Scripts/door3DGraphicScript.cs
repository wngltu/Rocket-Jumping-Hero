using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door3DGraphicScript : MonoBehaviour
{
    public void disableDoorGraphic()
    {
        this.gameObject.SetActive(false);
    }
    public void enableDoorGraphic()
    {
        this.gameObject.SetActive(true);
    }
}
