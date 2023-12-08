using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class DoorTimerVisualScript : MonoBehaviour
{
    float openTime;
    public float currentTime = 0f;
    public Slider doorTimer;
    // Start is called before the first frame update
    void Start()
    {
        doorTimer.gameObject.SetActive(false);
        openTime = GetComponentInParent<DoorMaster>().openTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime > 0f)
        {
            if (!doorTimer.gameObject.activeSelf)
                doorTimer.gameObject.SetActive(true);
            currentTime -= Time.deltaTime;
            doorTimer.value = currentTime / openTime;
        }
        if (currentTime < 0f)
        {
            currentTime = 0f;
            doorTimer.gameObject.SetActive(false);
        }
    }
}
