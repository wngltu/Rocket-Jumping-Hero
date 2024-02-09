using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class playerAnimationScript : MonoBehaviour
{
    public Animator anim;
    PlayerMovement playerMovementScript;
    public GameObject playerModel;

    bool facingLeft;
    // Start is called before the first frame update
    void Start()
    {
        playerMovementScript = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mousePosition.x < Screen.width / 2f)
        {
            facingLeft = true;
            playerModel.transform.localScale = new Vector3(1, 1, -1);
        }
        else
        {
            facingLeft = false;
            playerModel.transform.localScale = new Vector3(1, 1, 1);
        }

        anim.SetFloat("yMagnitude", playerMovementScript.controller.velocity.y);
        anim.SetBool("isGrounded", playerMovementScript.controller.isGrounded);
        anim.SetBool("shotRecently", playerMovementScript.shotRecently);

        if (!playerMovementScript.sprintHeld && facingLeft == false)
            anim.SetFloat("Input Magnitude", playerMovementScript.playerMovementVector.x); //Mathf.SmoothDamp(anim.GetFloat("Input Magnitude"), playerMovementScript.playerMovementVector.x, ref playerMovementScript.playerMovementVector.x, .5f);
        else if (playerMovementScript.sprintHeld && facingLeft == false)
            anim.SetFloat("Input Magnitude", playerMovementScript.playerMovementVector.x * 2);
        else if (!playerMovementScript.sprintHeld && facingLeft == true)
            anim.SetFloat("Input Magnitude", -playerMovementScript.playerMovementVector.x);
        else if (playerMovementScript.sprintHeld && facingLeft == true)
            anim.SetFloat("Input Magnitude", -playerMovementScript.playerMovementVector.x * 2);

        if (MathF.Abs(playerMovementScript.playerMovementVector.x) >= .01f)
        {
            anim.SetBool("isMoving", true);

            if (facingLeft == true && playerMovementScript.playerMovementVector.x < -.01f) //if player is moving and facing left
            {
                print("option1");
                anim.SetInteger("walkSpeedInt", 1);
                anim.SetBool("isMovingForward", true);
            }
            else if (facingLeft == true && playerMovementScript.playerMovementVector.x > .01f) //if player is facing left and moving right
            {
                print("option2");
                anim.SetInteger("walkSpeedInt", -1);
                anim.SetBool("isMovingForward", false);
            }
            else if (facingLeft == false && playerMovementScript.playerMovementVector.x < -.01f) //if player is facing right and moving left
            {
                print("option3");
                anim.SetInteger("walkSpeedInt", -1);
                anim.SetBool("isMovingForward", false);
            }
            else if (facingLeft == false && playerMovementScript.playerMovementVector.x > .01f) //if player is facing and moving right
            {
                print("option4");
                anim.SetInteger("walkSpeedInt", 1);
                anim.SetBool("isMovingForward", true);
            }
        }
        else
            anim.SetBool("isMoving", false);
    }
}
