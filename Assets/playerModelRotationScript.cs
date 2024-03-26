using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerModelRotationScript : MonoBehaviour
{
    public playerAnimationScript animScript;
    public GameObject RocketLauncher;

    Vector2 pointerPos;
    // Start is called before the first frame update
    void Start()
    {
        animScript = FindObjectOfType<playerAnimationScript>();
    }

    // Update is called once per frame
    void Update()
    {

            if (animScript.facingLeft) //if mouse on left half of screen
            {
                gameObject.transform.localScale = new Vector3(1, -1, 1);
            }
            else if (!animScript.facingLeft)//if mouse on right half of screen
            {
                gameObject.transform.localScale = Vector3.one;
            }

        gameObject.transform.rotation = RocketLauncher.transform.rotation;
    }
}
