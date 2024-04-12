using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class playerAnimationScript : MonoBehaviour
{
    public Animator anim;
    public Animator anim2D;
    PlayerMovement playerMovementScript;
    PlayerHealth playerHealthScript;
    PauseMenu pauseMenuScript;
    weaponManager weaponManagerScript;
    playerModelRotationScript rotScript;
    public GameObject playerModel;

    public bool facingLeft;
    // Start is called before the first frame update
    void Start()
    {
        playerMovementScript = FindObjectOfType<PlayerMovement>();
        playerHealthScript = FindAnyObjectByType<PlayerHealth>();
        pauseMenuScript = FindObjectOfType<PauseMenu>();
        weaponManagerScript = FindObjectOfType<weaponManager>();
        rotScript = FindObjectOfType<playerModelRotationScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mousePosition.x < Screen.width / 2f && pauseMenuScript.paused == false && !playerHealthScript.isDead)
        {
            facingLeft = true;
            playerModel.transform.localScale = new Vector3(1, 1, -1);
        }
        else if (Input.mousePosition.x >= Screen.width / 2f && pauseMenuScript.paused == false && !playerHealthScript.isDead)
        {
            facingLeft = false;
            playerModel.transform.localScale = new Vector3(1, 1, 1);
        }

        anim.SetFloat("yMagnitude", playerMovementScript.controller.velocity.y);
        anim.SetBool("isGrounded", playerMovementScript.controller.isGrounded);
        anim.SetBool("shotRecently", playerMovementScript.shotRecently);
        anim.SetBool("isReloading", weaponManagerScript.isReloading);
        if (playerHealthScript.isDead == true)
            anim.SetBool("isDead", true);

        if (!playerMovementScript.sprintHeld && facingLeft == false)
            anim.SetFloat("Input Magnitude", playerMovementScript.playerMovementVector.x); //Mathf.SmoothDamp(anim.GetFloat("Input Magnitude"), playerMovementScript.playerMovementVector.x, ref playerMovementScript.playerMovementVector.x, .5f);
        else if (playerMovementScript.sprintHeld && facingLeft == false)
            anim.SetFloat("Input Magnitude", playerMovementScript.playerMovementVector.x * 2);
        else if (!playerMovementScript.sprintHeld && facingLeft == true)
            anim.SetFloat("Input Magnitude", -playerMovementScript.playerMovementVector.x);
        else if (playerMovementScript.sprintHeld && facingLeft == true)
            anim.SetFloat("Input Magnitude", -playerMovementScript.playerMovementVector.x * 2);

        anim.SetFloat("aimAngle", rotScript.gameObject.transform.eulerAngles.z);

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

        if (weaponManagerScript.weaponInventory.Count == 0)
            anim.SetBool("hasNoWeapons", true);
        else
            anim.SetBool("hasNoWeapons", false);





        anim2D.SetBool("isGrounded", playerMovementScript.controller.isGrounded);
        if (MathF.Abs(playerMovementScript.playerMovementVector.x) >= .01f)
        {
            anim2D.SetBool("isMoving", true);
        }
        else
            anim2D.SetBool("isMoving", false);

        if (!playerMovementScript.sprintHeld && facingLeft == false)
            anim2D.SetFloat("Speed", playerMovementScript.playerMovementVector.x); //Mathf.SmoothDamp(anim.GetFloat("Input Magnitude"), playerMovementScript.playerMovementVector.x, ref playerMovementScript.playerMovementVector.x, .5f);
        else if (playerMovementScript.sprintHeld && facingLeft == false)
            anim2D.SetFloat("Speed", playerMovementScript.playerMovementVector.x * 2);
        else if (!playerMovementScript.sprintHeld && facingLeft == true)
            anim2D.SetFloat("Speed", -playerMovementScript.playerMovementVector.x);
        else if (playerMovementScript.sprintHeld && facingLeft == true)
            anim2D.SetFloat("Speed", -playerMovementScript.playerMovementVector.x * 2);
    }
}