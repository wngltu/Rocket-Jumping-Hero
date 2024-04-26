using UnityEngine;
using MonsterLove.StateMachine; //use _Enter, _Exit, _Finally for functions after the state enum
using System.Linq;
using Pathfinding;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;

public class FlyingMeleeEnemyFSM : Enemy
{
    public static FlyingMeleeEnemyFSM GruntInstance;
    float attackWindup = .75f;
    float attackTime = 2.5f;
    float attackCooldown = 2f;
    float attackRange = 10f;
    float baseDamage = 10f;

    float hoverDist = 2.5f; //this is the distance for flying enemies from the ground
    float hoverVeloCap = 3f;

    float idleTime = 3f;
    float patrolTime = 5f;


    float distanceFromPlayer;
    float timer;

    public bool recoiledRecently = false;
    bool isAttacking = true; //this is to make sure the enemy does not hit the player more than once in one attack

    public GameObject explodeIndicator;
    public AIDestinationSetter aiDestinationSetter;
    public States currentState;
    public AudioSource attackWindUpSound;
    public AudioSource attackSound;
    public SpriteRenderer sprite;
    public Sprite idleModel;
    public Sprite attackingModel;
    public Sprite cooldownModel;
    public Sprite windupModel;
    public ParticleSystem thruster;

    LayerMask LoSLayerMask = ((1 << 3) | (1 << 12)); //this is for checking line of sight
    LayerMask HoverLayerMask = ((1 << 12)); //this is for checking for ground to hover on

    Vector3 attackDirection;

    void Test_Update()
    {
        Debug.Log("Test UPDATE");
    }

    public enum States
    {
        Init,
        Idle,
        Patrol,
        Attack,
        AttackCooldown,
        Chasing,
        AttackWindup,
        Die
    }
    float aggroRange = 20f;
    float stateTime = 0f;

    StateMachineRunner stateMachineRunner;
    PlayerMovement playerMovement;

    private void Start()
    {
        fsm = new StateMachine<States, StateDriverUnity>(this);
        fsm.ChangeState(States.Init, StateTransition.Safe);


        base.Start();
        base.weaponLoot = base.prefabLoot.pistolDrop;
        GruntInstance = this;

        if (aiDestinationSetter == null)
            aiDestinationSetter = GetComponent<AIDestinationSetter>();
        if (playerMovement == null)
            playerMovement = FindObjectOfType<PlayerMovement>();
        if (stateMachineRunner == null)
            stateMachineRunner = FindObjectOfType<StateMachineRunner>();

        maxVelocityX = 10f;
    }

    private StateMachine<States, StateDriverUnity> fsm;


    private void Update()
    {
        base.Update();
        fsm.Driver.Update.Invoke();
        currentState = fsm.State;

        if (Died == true)
            fsm.ChangeState(States.Die, StateTransition.Safe);
    }

    void Init_Enter()
    {
        Debug.Log("enter init state");
        fsm.ChangeState(States.Idle, StateTransition.Safe);
    }
    void Init_Exit()
    {
        Debug.Log("exit init state");
    }

    void Idle_Enter()
    {
        sprite.sprite = idleModel;
        thruster.Stop();
        aiPath.enabled = false;
        stateTime = 0;
        Debug.Log("enter idle state");
        aiDestinationSetter.target = this.gameObject.transform;
    }

    void Idle_Update()
    {
        stateTime += Time.deltaTime;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, hoverDist, HoverLayerMask) && rb.velocity.y < hoverVeloCap)
        {
            rb.velocity += new Vector3(0, .1f, 0);
        }
        else if (rb.velocity.y > -hoverVeloCap)
            rb.velocity -= new Vector3(0, .1f, 0);
        if (stateTime > idleTime)
            fsm.ChangeState(States.Patrol, StateTransition.Safe);
    }

    void Idle_Exit()
    {
        aiPath.enabled = true;
    }

    void Patrol_Enter()
    {
        sprite.sprite = idleModel;
        thruster.Play();
        stateTime = 0;
        WalkToRandomNearbyNode();
        Debug.Log("enter patrol state");
    }


    void Patrol_Update()
    {
        stateTime += Time.deltaTime;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, hoverDist, HoverLayerMask) && rb.velocity.y < hoverVeloCap)
        {
            rb.velocity += new Vector3(0, .1f, 0);
        }
        else if (rb.velocity.y > -hoverVeloCap)
            rb.velocity -= new Vector3(0, .1f, 0);
        if (stateTime > patrolTime)
        {
            fsm.ChangeState(States.Idle, StateTransition.Safe);
        }

        if (rb.velocity.x > 0.1f)
            model.transform.localScale = new Vector3(-1, 1, 1);
        else if (rb.velocity.x < -.1f)
            model.transform.localScale = new Vector3(1, 1, 1);
    }

    void Chasing_Enter()
    {
        sprite.sprite = idleModel;
        thruster.Play();
        aiDestinationSetter.target = playerMovement.transform;
    }

    void Chasing_Update()
    {
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceFromPlayer < attackRange) //is enemy close enough to start attack
        {
            fsm.ChangeState(States.AttackWindup, StateTransition.Safe);
        }
        if (stateTime > patrolTime)
        {
            fsm.ChangeState(States.Idle, StateTransition.Safe);
        }

        if (playerToTheRight)
            model.transform.localScale = new Vector3(-1, 1, 1);
        else
            model.transform.localScale = new Vector3(1, 1, 1);
    }

    void Chasing_Exit()
    {
        aiDestinationSetter.target = null;
    }

    void AttackWindup_Enter()
    {
        sprite.sprite = windupModel;
        thruster.Stop();
        rb.velocity = Vector3.zero;
        attackWindUpSound.Play();
        UpdatePlayerDirection();
        if (playerToTheRight == true)
        {
            model.transform.localScale = new Vector3(-1, 1, 1);
            rb.AddForce(new Vector2(-1, .5f), ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(new Vector2(1, .5f), ForceMode.Impulse);
            model.transform.localScale = new Vector3(1, 1, 1);
        }
        aiPath.enabled = false;
        timer = attackWindup;
    }
    void AttackWindup_Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = 0;
            rb.AddForce(0, -100f, 0);
            fsm.ChangeState(States.Attack, StateTransition.Safe);
        }
    }

    bool chargeRight;
    void Attack_Enter()
    {
        thruster.Play();
        sprite.sprite = attackingModel;
        attackSound.Play();
        attacking = true;
        timer = 0f;
        timer = attackTime;
        isAttacking = true;
        Debug.Log("start attacK");
        UpdatePlayerDirection();
        if (playerToTheRight == true)
            chargeRight = true;
        else
            chargeRight = false;
    }
    void Attack_Update()
    {
        if (timer >= attackTime / 2) // first half of attack duration, speed up
        {
            if (chargeRight)
                rb.velocity += new Vector3(.25f, 0, 0);
            else if (!chargeRight)
                rb.velocity += new Vector3(-.25f, 0, 0);
        }
        if (timer < attackTime / 2) //second half of attack duration, slow down
        {
            if (chargeRight && rb.velocity.x > 0)
            {
                rb.velocity += new Vector3(-.25f, 0, 0);
            }
            else if (!chargeRight && rb.velocity.x < 0)
                rb.velocity += new Vector3(.25f, 0, 0);

            if (chargeRight && rb.velocity.x < 0)
                rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
            else if (!chargeRight && rb.velocity.x > 0)
                rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
        }
        timer -= Time.deltaTime;

        if (rb.velocity.x > maxVelocityX)
            rb.velocity = new Vector3(maxVelocityX, rb.velocity.y, rb.velocity.z);
        if (rb.velocity.x < -maxVelocityX)
            rb.velocity = new Vector3(-maxVelocityX, rb.velocity.y, rb.velocity.z);

        if (timer < 0 && rb.velocity.x == 0)
        {
            timer = 0;
            fsm.ChangeState(States.AttackCooldown, StateTransition.Safe);
        }
    }
    void Attack_Exit()
    {
        attacking = false;
        isAttacking = false;
    }

    void AttackCooldown_Enter()
    {
        sprite.sprite = cooldownModel;
        thruster.Stop();
        timer = 0f;
        timer = attackCooldown;
    }
    void AttackCooldown_Update()
    {
        timer -= Time.deltaTime;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, hoverDist, HoverLayerMask) && rb.velocity.y < hoverVeloCap)
        {
            rb.velocity += new Vector3(0, .1f, 0);
        }
        else if (rb.velocity.y > -hoverVeloCap)
            rb.velocity -= new Vector3(0, .1f, 0);
        if (timer < 0)
        {
            timer = 0;
            fsm.ChangeState(States.Idle, StateTransition.Safe);
        }
    }

    void Die_Enter()
    {
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && (fsm.State == States.Idle || fsm.State == States.Patrol))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, GetVectorToPlayer(), out hit, 99, LoSLayerMask))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    aggroed = true;
                    fsm.ChangeState(States.Chasing, StateTransition.Safe);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && (fsm.State == States.Idle || fsm.State == States.Patrol))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, GetVectorToPlayer(), out hit, 99, LoSLayerMask))
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    aggroed = true;
                    fsm.ChangeState(States.Chasing, StateTransition.Safe);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            aggroed = false;
            fsm.ChangeState(States.AttackCooldown, StateTransition.Safe);
        }
    }

    void WalkToRandomNearbyNode()
    {
        Collider[] cols = Physics.OverlapSphere(this.transform.position, aggroRange);
        List<Collider> pathingNodes = new List<Collider>();
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("pathingnode")) //is the collider a pathing node? add to vector of nearby nodes
            {
                pathingNodes.Add(col);
            }
        }
        pathingNodes.Append(this.gameObject.GetComponent<Collider>());
        aiDestinationSetter.target = pathingNodes[Random.Range(0, pathingNodes.Count - 1)].transform; //set new target to a random nearby node's position
        Debug.Log("finished calculating");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isAttacking == true)
        {
            isAttacking = false;
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(15);
            print(collision.gameObject.name);
            fsm.ChangeState(States.AttackCooldown, StateTransition.Safe);
        }
    }
}
