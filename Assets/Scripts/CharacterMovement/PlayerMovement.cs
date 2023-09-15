using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Component variables
    public CharacterController controller;
    public BoxCollider jumpCheck;
    public TrailRenderer trail;
    public static PlayerMovement Instance;
    
    //Horizontal variables
    private float horizontalVeloCap = 10f;
    private float groundedPlayerSpeed = 6.5f;
    private float aerialPlayerSpeed = 5f;
    private float playerSpeed = 6.5f;
    private float playerSprintSpeed = 10f;

    //Vertical/Jump variables
    private float gravity = -17f;
    private float defaultGravity = -17f;
    private float rocketJumpingGravity = -15f;
    private float jumpStrength = 7.5f;
    private float jumpCooldown = 0;
    private float multiJumpCooldown = .1f;
    private float jumpsLeft = 2;
    private float verticalVeloCap = 10f;
    private float defaultVerticalVeloCap = 10f;
    private float rocketJumpingVerticalVeloCap = 12f;

    //Dash variables
    private float dashCooldown = 3; //the set limit cooldown of dash
    private float dashTimer = 0f; //the current cooldown of the player's dash
    private float dashStrength = 15f; //how strong the dash is
    private float doubleTapTimer = 0f; //the leniency for double tapping shift to dash
    private float doubleTapLeniency = .25f; //the input window for double tapping shift (higher = more lenient)

    //Other variables
    private float explosionRecoilRecoveryRate = 10; //higher = faster recovery
   
    Vector2 velocity;

    //Bools
    bool canDash = true;
    bool grounded;
    bool sprintHeld = false;
    bool jumpHeld = false;
    bool facingRight = true;
    bool isRocketJumping = false;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        jumpHeld = Input.GetKeyDown(KeyCode.Space);

        if (canDash == true && doubleTapTimer > 0 && Input.GetKeyDown(KeyCode.LeftShift)) //initiate dash
        {
            canDash = false;
            dashTimer = dashCooldown;
            print("Player dashed");
            StartTrail();

            if (facingRight == true) //Right side dash
            {
                velocity.x += dashStrength;
            }
            else if (facingRight == false) //left side dash
            {
                velocity.x -= dashStrength;
            }
        }

        if (dashTimer > 0) //if dash is on cooldown, decrease cooldown over time
        {
            dashTimer = dashTimer - Time.deltaTime;
        }
        else if (dashTimer < 0) //if dash is not on cooldown, set can dash to true
        {
            dashTimer = 0;
            canDash = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            sprintHeld = true;
            if(canDash == true)
            {
                doubleTapTimer = doubleTapLeniency; //initiate input window for dash
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            sprintHeld = false;
        }

        if (Input.GetKeyDown(KeyCode.A) && !Input.GetKeyDown(KeyCode.D)) { facingRight = false; }
        else if (Input.GetKeyDown(KeyCode.D) && !Input.GetKey(KeyCode.A)) { facingRight = true; }

        if (doubleTapTimer > 0f)
        {
            doubleTapTimer -= Time.deltaTime;
        }
        else if (doubleTapTimer < 0f)
        {
            doubleTapTimer = 0;
        }

        grounded = controller.isGrounded;
        if (grounded && playerSpeed != groundedPlayerSpeed && isRocketJumping == false)
        {
            SetNormalAerialValues();
        }

        if (jumpCooldown < 0) //Single jump cooldown
        {
            jumpCooldown = 0;
        }
        else if (jumpCooldown > 0) 
        {
            jumpCooldown -= Time.deltaTime;
        }

        if (multiJumpCooldown < 0) //Double jump cooldown
        {
            multiJumpCooldown = 0;
        }
        else if (multiJumpCooldown > 0) 
        {
            multiJumpCooldown -= Time.deltaTime;
        }


        if (grounded && velocity.y < 0) //If the player lands, they can jump again
        {
            velocity.y = -.5f;
            jumpsLeft = 2;
        }

        if ((jumpHeld && grounded && jumpCooldown <= 0) || (jumpHeld && jumpsLeft > 0 && multiJumpCooldown <= 0)) //Is the player able to double jump again
        {
            velocity.y += jumpStrength;
            if (velocity.y > verticalVeloCap)
            {
                velocity.y = verticalVeloCap;
            }
            jumpCooldown = .5f;
            jumpsLeft -= 1;
        }

        if (velocity.x > .1f) //Velocity is from rocket jumping, so this is slowing the effects of rocket jumping on the player
        {
            velocity.x -= Time.deltaTime * explosionRecoilRecoveryRate;
        }
        else if (velocity.x < -.1f)
        {
            velocity.x += Time.deltaTime * explosionRecoilRecoveryRate;
            //velocity.x = Mathf.Lerp(velocity.x, 0, 5f);
        }
        else
        {
            velocity.x = 0;
        }

        if (velocity.x > horizontalVeloCap) //if player is moving too fast to the right, cap the players velocity
        {
            velocity.x = horizontalVeloCap;
        }
        else if (velocity.x < -horizontalVeloCap) //if player is moving too fast to the left, cap the players velocity
        {
            velocity.x = -horizontalVeloCap;
        }

        if (velocity.y < -verticalVeloCap*1.25f) //if player is moving too fast downwards, cap the player's velocity
        {
            velocity.y = -verticalVeloCap*1.25f;
        }

        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), 0); //horizontal movement
        if (sprintHeld && grounded) //if sprinting, move at sprint speed
        {
            controller.Move(move * Time.deltaTime * playerSprintSpeed);
        }
        else if (!sprintHeld && grounded)//if not sprinting, move at normal speed
        {
            controller.Move(move * Time.deltaTime * playerSpeed);
        }
        else if (sprintHeld && !grounded)
        {
            controller.Move(move * Time.deltaTime * playerSprintSpeed/2);
        }
        else if (!sprintHeld && !grounded)
        {
            controller.Move(move * Time.deltaTime * playerSpeed/2);

        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void AddExplosionForce(Vector2 explosionPos, float explosionRadius, float explosionForce)
    {
        Vector2 newVector = new Vector2(transform.position.x, transform.position.y);
        Vector2 explosionVec = newVector - explosionPos; // vector from explosion to our center
        float distance = explosionVec.magnitude; // get distance to explosion

        if (distance > explosionRadius) return; // we're outside the explosion's radius, ignore it

        float forceMult = 1f - (distance / explosionRadius); // 1.0 at explosion center, 0.0 at explosion radius

        explosionVec /= distance; // normalize the vector
        this.velocity.y += (explosionVec * explosionForce * forceMult).y; // add explosion impulse to velocity
        this.velocity.x += (explosionVec * explosionForce * forceMult).x * 1.5f;
        if ((explosionVec * explosionForce * forceMult).magnitude > 10f) //edit right side to change the threshold for which trail to start appearing after an explosion of variable strength
        {
            StartTrail();
            SetRocketJumpAerialValues();
            setRocketJumpingOn();
        }
        if (velocity.y > verticalVeloCap)
        {
            velocity.y = verticalVeloCap;
        }
    }

    public void setRocketJumpingOn()
    {
        isRocketJumping = true;
        playerSpeed = 0f;
        Invoke("setRocketJumpingOff", .25f);
    }

    private void setRocketJumpingOff()
    {
        if (grounded)
            playerSpeed = groundedPlayerSpeed;
        else
            playerSpeed = aerialPlayerSpeed;
        isRocketJumping = false;
    }

    public void SetNormalAerialValues() //call when landing
    {
        playerSpeed = groundedPlayerSpeed;
        gravity = defaultGravity;
        verticalVeloCap = defaultVerticalVeloCap;
        isRocketJumping = false;
    }

    public void SetRocketJumpAerialValues() // call when being hit by an explosion
    {
        playerSpeed = aerialPlayerSpeed;
        gravity = rocketJumpingGravity;
        verticalVeloCap = rocketJumpingVerticalVeloCap;
        isRocketJumping = true;
    }

    public void StartTrail()
    {
        Invoke("StopTrail", .25f);
        trail.emitting = true;
    }

    void StopTrail() //invoke to stop the trail 
    {
        trail.emitting = false;
    }
}