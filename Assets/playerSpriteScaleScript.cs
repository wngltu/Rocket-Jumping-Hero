using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSpriteScaleScript : MonoBehaviour
{
    public GameObject model;
    public GameObject hand;
    public GameObject armPivot;
    public Camera playerCam;
    public PauseMenu pauseManager;
    public bool directionRight = true;

    public Vector3 originalScale;
    public Vector3 originalHandScale;
    public Vector2 pointerPos;
    public Vector2 targetPos;
    // Start is called before the first frame update
    void Start()
    {
        pauseManager = FindAnyObjectByType<PauseMenu>();
        playerCam = FindObjectOfType<PlayerZoomManager>().gameObject.GetComponent<Camera>();
        originalScale = model.transform.localScale;
        originalHandScale = hand.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!pauseManager.paused)
        {
            pointerPos = Input.mousePosition;
            pointerPos = playerCam.ScreenToWorldPoint(pointerPos);
            pointerPos = pointerPos - (Vector2)transform.position;
            targetPos = pointerPos;
            armPivot.transform.right = pointerPos;

            if (Input.mousePosition.x < Screen.width / 2f) //if mouse on left half of screen
            {
                model.transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
                hand.transform.localScale = new Vector3(originalHandScale.x, originalHandScale.y, originalHandScale.z);
                armPivot.transform.localScale = new Vector3(-1, -1, 1);
            }
            else //if mouse on right half of screen
            {
                model.transform.localScale = originalScale;
                hand.transform.localScale = originalHandScale;
                armPivot.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
