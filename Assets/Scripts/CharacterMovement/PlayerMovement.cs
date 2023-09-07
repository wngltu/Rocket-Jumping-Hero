using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public BoxCollider jumpCheck;
    public static PlayerMovement Instance;

    private float playerSpeed = 6.5f;
    private float gravity = -15f;
    private float jumpStrength = 7.5f;
    private float jumpCooldown = 0f;
    private float multiJumpCooldown = .1f;
    private float jumpsLeft = 2;
    private float explosionRecoilRecoveryRate = 10; //higher = faster recovery
    private float horizontalVeloCap = 7.5f;
    private float verticalVeloCap = 10f;

    Vector2 velocity;

    bool grounded;
    bool jumpHeld = false;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        jumpHeld = Input.GetKeyDown(KeyCode.Space);

        grounded = controller.isGrounded;

        if (jumpCooldown < 0)
        {
            jumpCooldown = 0;
        }
        else if (jumpCooldown > 0)
        {
            jumpCooldown -= Time.deltaTime;
        }

        if (multiJumpCooldown < 0)
        {
            multiJumpCooldown = 0;
        }
        else if (multiJumpCooldown > 0)
        {
            multiJumpCooldown -= Time.deltaTime;
        }


        if (grounded && velocity.y < 0)
        {
            velocity.y = -.5f;
            jumpsLeft = 2;
        }
        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), 0);
        controller.Move(move * Time.deltaTime * playerSpeed);

        if ((jumpHeld && grounded && jumpCooldown <= 0) || (jumpHeld && jumpsLeft > 0 && multiJumpCooldown <= 0))
        {
            velocity.y += jumpStrength;
            if (velocity.y > verticalVeloCap)
            {
                velocity.y = verticalVeloCap;
            }
            jumpCooldown = .5f;
            jumpsLeft -= 1;
        }

        if (velocity.x > .1f)
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

        if (velocity.x > horizontalVeloCap)
        {
            velocity.x = horizontalVeloCap;
        }
        else if (velocity.x < -horizontalVeloCap)
        {
            velocity.x = -horizontalVeloCap;
        }



        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("ground") == true)
        {
            grounded = true;
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
        this.velocity += explosionVec * explosionForce * forceMult; // add explosion impulse to velocity
        print("swaws");
        if (velocity.y > verticalVeloCap)
        {
            velocity.y = verticalVeloCap;
        }
    }
}