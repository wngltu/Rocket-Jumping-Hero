using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    //Component variables
    public CharacterController controller;
    public BoxCollider jumpCheck;
    public TrailRenderer trail;
    public static PlayerMovement Instance;
    public TMP_Text rocketText;
    public TMP_Text slowmoStatusText;
    public Slider rocketReloadSlider;
    public weaponManager weaponManager;
    public PlayerRocketLauncher rocketLauncher;
    public PauseMenu pauseManager; 

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
    private float jumpsLeft = 1;
    private float verticalVeloCap = 10f;
    private float defaultVerticalVeloCap = 10f;
    private float rocketJumpingVerticalVeloCap = 12f;
    private float ceilingVelocity = 2f;

    //Dash variables
    private float dashCooldown = 3; //the set limit cooldown of dash
    private float dashTimer = 0f; //the current cooldown of the player's dash
    private float dashStrength = 15f; //how strong the dash is
    private float doubleTapATimer = 0f; //the leniency for double tapping a to dash
    private float doubleTapALeniency = .25f; //the input window for double tapping a (higher = more lenient)
    private float doubleTapDTimer = 0f; //the leniency for double tapping d to dash
    private float doubleTapDLeniency = .25f; //the input window for double tapping d (higher = more lenient)

    //Rocket Launcher variables
    private int rockets = 4;
    private int maxRockets = 4;
    private float rocketRegenCooldown = 1.5f;
    private float rocketRegenTimer = 0f;
    private float rocketTapWindow = .15f;
    private float rocketTapTimer = 0f;
    public float rocketFireRate = .15f;
    public float rocketFireCooldownTimer = .0f;

    //Other variables
    private float explosionRecoilRecoveryRate = 10; //higher = faster recovery

    Vector2 velocity;

    //Bools
    bool canDash = true;
    public bool grounded;
    bool sprintHeld = false;
    bool jumpHeld = false;
    bool facingRight = true;
    bool isRocketJumping = false;
    public bool isJumping;
    public bool slowmoEnabled = false;
    // Start is called before the first frame update

    void Start()
    {
        controller.enabled = false; //unity character controller is.... VERY INCONVENIENT.
        transform.position = new Vector3(SaveData.checkpointX, SaveData.checkpointY, 0);
        controller.enabled = true;
        Instance = this;
        rocketText.text = rockets.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        jumpHeld = Input.GetKeyDown(KeyCode.Space);

        if (canDash == true && doubleTapATimer > 0 && Input.GetKeyDown(KeyCode.A)) //initiate dash
        {
            canDash = false;
            dashTimer = dashCooldown;
            print("Player dashed left");
            StartTrail();

            velocity.x -= dashStrength;
        }

        if (canDash == true && doubleTapDTimer > 0 && Input.GetKeyDown(KeyCode.D))
        {
            canDash = false;
            dashTimer = dashCooldown;
            print("Player dashed right");
            StartTrail();

            velocity.x += dashStrength;
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

        if (Input.GetKeyDown(KeyCode.A) && canDash == true) //left dash
        {
            doubleTapATimer = doubleTapALeniency; //initiate input window for dash
        }
        else if (Input.GetKeyDown(KeyCode.D) && canDash == true) //right dash
        {
            doubleTapDTimer = doubleTapDLeniency;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
            sprintHeld = true;
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            sprintHeld = false;

        if (Input.GetKeyDown(KeyCode.A) && !Input.GetKeyDown(KeyCode.D)) { facingRight = false; }
        else if (Input.GetKeyDown(KeyCode.D) && !Input.GetKey(KeyCode.A)) { facingRight = true; }

        if (doubleTapATimer > 0f)
            doubleTapATimer -= Time.deltaTime;
        else if (doubleTapATimer < 0f)
            doubleTapATimer = 0;

        if (doubleTapDTimer > 0f)
            doubleTapDTimer -= Time.deltaTime;
        else if (doubleTapDTimer < 0f)
            doubleTapDTimer = 0;

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
            jumpsLeft = 1;
        }

        if ((jumpHeld && grounded && jumpCooldown <= 0) || (jumpHeld && jumpsLeft > 0 && multiJumpCooldown <= 0)) //Is the player able to double jump again
        {
            if (velocity.y < 0)
                velocity.y = jumpStrength;
            else
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

        if (velocity.y < -verticalVeloCap * 1.25f) //if player is moving too fast downwards, cap the player's velocity
        {
            velocity.y = -verticalVeloCap * 1.25f;
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
            controller.Move(move * Time.deltaTime * playerSprintSpeed / 2);
        }
        else if (!sprintHeld && !grounded)
        {
            controller.Move(move * Time.deltaTime * playerSpeed / 2);

        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (rocketFireCooldownTimer > 0)
            rocketFireCooldownTimer -= Time.deltaTime;
        else if (rocketFireCooldownTimer < 0)
            rocketFireCooldownTimer = 0;


        if (Input.GetKey(KeyCode.Mouse1) && pauseManager.paused == false) //if player starts holding right click, start rocket "aim mode"
        {
            weaponManager.UnequipAll();
            rocketLauncher.Equip();
            if (slowmoEnabled == true)
                Time.timeScale = .3f;
            if (Input.GetKeyDown(KeyCode.Mouse0) && rockets > 0 && rocketFireCooldownTimer == 0) //shoot rocket upon clicking left click
            {
                rocketLauncher.Shoot();
                rockets--;
                rocketText.text = rockets.ToString();
                rocketReloadSlider.value = rocketRegenTimer / rocketRegenCooldown;
                rocketTapTimer = 0f; //make sure player can't tap shoot after shooting a held rocket
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && pauseManager.paused == false) //when player clicks button, start rocket tap input window
            rocketTapTimer = rocketTapWindow;
        else if (Input.GetKeyUp(KeyCode.Mouse1) && pauseManager.paused == false)
        {
            rocketLauncher.Unequip();
            weaponManager.EquipCurrentWeapon();
            if (slowmoEnabled == true)
                Time.timeScale = 1f;
        }

        if (Input.GetKeyUp(KeyCode.Mouse1) && rocketTapTimer > 0 && rockets > 0 && pauseManager.paused == false && rocketFireCooldownTimer == 0) //if rocket tap input window did not elapse and player "taps" and lets go, execute rocket shoot
        {
            rocketLauncher.Shoot();
            rockets--;
            rocketText.text = rockets.ToString();
            rocketReloadSlider.value = rocketRegenTimer / rocketRegenCooldown;
            rocketTapTimer = 0;
        }

        if (rocketTapTimer > 0)
            rocketTapTimer -= Time.deltaTime;
        else if (rocketTapTimer < 0)
            rocketTapTimer = 0f;
            rocketTapTimer = 0f;
                

        if (grounded && rockets < maxRockets) //"reload"/regenerate rockets while standing on the ground
        {
            rocketRegenTimer += Time.deltaTime;
            rocketReloadSlider.value = rocketRegenTimer / rocketRegenCooldown;
            if (rocketRegenTimer > rocketRegenCooldown)
            {
                rockets++;
                rocketText.text = rockets.ToString();
                rocketRegenTimer = 0;
            }
        }
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

    public void toggleSlowMotionAim()
    {
        slowmoEnabled = !slowmoEnabled;
        if (slowmoEnabled == true)
            slowmoStatusText.text = "On";
        if (slowmoEnabled == false)
        {
            slowmoStatusText.text = "Off";
            Time.timeScale = 1f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("platform"))
        {
            if (other.GetComponent<Platform>().col.enabled == true && Input.GetKey(KeyCode.S)) //Drop through platforms
            {
                other.GetComponent<Platform>().disablePlayerCollisions();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ground"))
        {
            velocity = new Vector3(controller.velocity.x, .5f, controller.velocity.z);
        }
    }
}