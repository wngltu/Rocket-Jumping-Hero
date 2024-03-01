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
    public PlayerHealth playerHealth;
    public BoxCollider jumpCheck;
    public TrailRenderer trail;
    public static PlayerMovement Instance;
    public TMP_Text rocketText;
    public TMP_Text slowmoStatusText;
    public Slider rocketReloadSlider;
    public weaponManager weaponManager;
    public PlayerRocketLauncher rocketLauncher;
    public PauseMenu pauseManager;
    public AudioSource jumpSound;
    public AudioSource jumpLandSound;
    public AudioSource dashSound;
    public GameObject respawnVFX;
    public GameObject playerModel;
    public AudioSource respawnSFX;

    //Horizontal variables
    private float horizontalVeloCap = 10f;
    private float groundedPlayerSpeed = 6.5f;
    private float aerialPlayerSpeed = 7.5f;
    private float aerialPlayerSprintSpeed = 12f;
    private float playerSpeed = 6.5f;
    public float playerSprintSpeed = 10f;
    private float groundedSprintSpeed = 10f;

    //Vertical/Jump variables
    private float gravity = -16f;
    private float defaultGravity = -16f;
    private float rocketJumpingGravity = -14f;
    private float jumpStrength = 7.5f;
    private float jumpCooldown = 0;
    private float multiJumpCooldown = .1f;
    public float jumpsLeft = 1;
    public float jumpsCap = 2;
    private float verticalVeloCap = 8f;
    private float defaultVerticalVeloCap = 8f;
    private float rocketJumpingVerticalVeloCap = 10f;
    private float ceilingVelocity = 2f;
    private float landSFXCooldownTimer = 0;
    private float landSFXCooldown = .5f;
    private float airDuration = 0f;

    //Dash variables
    private float dashCooldown = 3; //the set limit cooldown of dash
    private float dashTimer = 0f; //the current cooldown of the player's dash
    private float dashStrength = 15f; //how strong the dash is
    private float doubleTapATimer = 0f; //the leniency for double tapping a to dash
    private float doubleTapALeniency = .25f; //the input window for double tapping a (higher = more lenient)
    private float doubleTapDTimer = 0f; //the leniency for double tapping d to dash
    private float doubleTapDLeniency = .25f; //the input window for double tapping d (higher = more lenient)

    //Rocket Launcher variables
    public int rockets = 4;
    public int maxRockets = 4;
    private float rocketRegenCooldown = 1.5f;
    private float rocketRegenTimer = 0f;
    private float rocketTapWindow = 0f;
    private float rocketTapTimer = 0f;
    public float rocketFireRate = .35f;
    public float rocketFireCooldownTimer = .0f;
    public float rocketRecentlyShotTimer = 0f;
    public float rocketRecentlyShotTime = 1.5f;

    //Other variables
    private float explosionRecoilRecoveryRate = 10; //higher = faster recovery
    private float shotRecentlyTimer = 0;
    private float shotRecentlyTime = .75f;

    Vector2 velocity;
    public Vector2 playerMovementVector;

    //Bools
    bool canDash = true;
    public bool grounded;
    public bool sprintHeld = false;
    bool jumpHeld = false;
    bool facingRight = true;
    public bool isRocketJumping = false;
    public bool isJumping;
    public bool slowmoEnabled = false;
    bool wasInAir = true; //used to play jump land sfx
    public bool shotRecently = false;
    public bool isRespawning = false;
    bool decreasedJump = false;
    // Start is called before the first frame update

    void Start()
    {
        controller.enabled = false; //unity character controller is.... VERY INCONVENIENT.
        transform.position = new Vector3(SaveData.checkpointX, SaveData.checkpointY, 0);
        controller.enabled = true;
        pauseManager = FindObjectOfType<PauseMenu>();
        playerHealth = GetComponent<PlayerHealth>();
        Instance = this;
        rocketText.text = rockets.ToString();
        StartSpawnSequence();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.z != 0)
        {
            gameObject.GetComponent<CharacterController>().enabled = false;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);
            gameObject.GetComponent<CharacterController>().enabled = true;
        }

        jumpHeld = Input.GetKeyDown(KeyCode.Space);

        if (canDash == true && doubleTapATimer > 0 && Input.GetKeyDown(KeyCode.A)) //initiate dash
        {
            canDash = false;
            dashTimer = dashCooldown;
            print("Player dashed left");
            StartTrail();

            velocity.x -= dashStrength;
            dashSound.Play();
            
        }

        if (canDash == true && doubleTapDTimer > 0 && Input.GetKeyDown(KeyCode.D))
        {
            canDash = false;
            dashTimer = dashCooldown;
            print("Player dashed right");
            StartTrail();

            velocity.x += dashStrength;
            dashSound.Play();
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

        if (grounded && wasInAir == true && landSFXCooldownTimer == 0 && airDuration > .2f)
        {
            wasInAir = false;
            landSFXCooldownTimer = landSFXCooldown;
            jumpLandSound.Play();
        }
        if (!grounded && wasInAir == false)
        {
            wasInAir = true;
        }

        if (landSFXCooldownTimer > 0f)
            landSFXCooldownTimer -= Time.deltaTime;
        if (landSFXCooldownTimer < 0)
            landSFXCooldownTimer = 0;

        if (!grounded)
            airDuration += Time.deltaTime;
        if (grounded)
            airDuration = 0;

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
            jumpsLeft = jumpsCap;
            decreasedJump = false;
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
            jumpSound.Play();
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
        playerMovementVector = move;
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
            controller.Move(move * Time.deltaTime * playerSprintSpeed);
        }
        else if (!sprintHeld && !grounded)
        {
            controller.Move(move * Time.deltaTime * playerSpeed);

        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (rocketFireCooldownTimer > 0)
            rocketFireCooldownTimer -= Time.deltaTime;
        else if (rocketFireCooldownTimer < 0)
            rocketFireCooldownTimer = 0;


        if (Input.GetKey(KeyCode.Mouse1) && pauseManager.paused == false && !isRespawning) //if player starts holding right click, start rocket "aim mode"
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
        //if (Input.GetKeyDown(KeyCode.Mouse1) && pauseManager.paused == false) //when player clicks button, start rocket tap input window
        //    rocketTapTimer = rocketTapWindow;
        else if (Input.GetKeyUp(KeyCode.Mouse1) && pauseManager.paused == false)
        {
            rocketLauncher.Unequip();
            if (weaponManager.weaponInventory.Count > 0)
            {
                weaponManager.EquipCurrentWeapon();
            }
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

        if (rocketRecentlyShotTimer > 0)
            rocketRecentlyShotTimer -= Time.deltaTime;
        else if (rocketRecentlyShotTimer < 0)
            rocketRecentlyShotTimer = 0f;

        if (grounded && rockets < maxRockets) //"reload"/regenerate rockets while standing on the ground
        {
            if (Mathf.Abs(playerMovementVector.x) > 0 || rocketRecentlyShotTimer != 0) //if player is moving, regen at normal rate
                rocketRegenTimer += Time.deltaTime;
            else //if player is not moving, regen at 1.5x rate
                rocketRegenTimer += Time.deltaTime * 2;
            rocketReloadSlider.value = rocketRegenTimer / rocketRegenCooldown;
            if (rocketRegenTimer > rocketRegenCooldown)
            {
                rockets++;
                rocketText.text = rockets.ToString();
                rocketRegenTimer = 0;
            }
        }

        if (shotRecentlyTimer > 0)
        {
            shotRecentlyTimer -= Time.deltaTime;
            shotRecently = true;
        }
        else if (shotRecentlyTimer < 0)
        {
            shotRecentlyTimer = 0f;
            shotRecently = false;
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
        playerSprintSpeed = groundedSprintSpeed;
        gravity = defaultGravity;
        verticalVeloCap = defaultVerticalVeloCap;
        isRocketJumping = false;
    }

    public void SetRocketJumpAerialValues() // call when being hit by an explosion
    {
        playerSpeed = aerialPlayerSpeed;
        playerSprintSpeed = aerialPlayerSprintSpeed;
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

    public void setShotRecentlyTimer()
    {
        shotRecentlyTimer = shotRecentlyTime;
    }

    void StartSpawnSequence()
    {
        respawnSFX.Play();
        isRespawning = true;
        controller.enabled = false;
        playerModel.SetActive(false);
        GameObject newObj = Instantiate(respawnVFX, transform);
        newObj.transform.SetParent(null);
        Invoke("EndSpawnSequence", 1);
    }

    public void DecreaseJumpsLeft() //decrease jumps on first rocket jump, so the normal grounded jump cannot be performed
    {
        if (!decreasedJump)
        {
            jumpsLeft--;
            decreasedJump = true;
        }
    }

    void EndSpawnSequence()
    {
        isRespawning = false;
        controller.enabled = true;
        playerModel.SetActive(true);
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