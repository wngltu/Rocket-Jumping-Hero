using UnityEngine;
using MonsterLove.StateMachine; //use _Enter, _Exit, _Finally for functions after the state enum
using System.Linq;
using Pathfinding;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;

public class MeleeGruntFSM : Enemy
{
    public static MeleeGruntFSM GruntInstance;
    float attackWindup = .75f;
    float attackTime = 1.5f;
    float attackCooldown = 2f;
    float attackRange = 3.5f;
    float baseDamage = 10f;

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

    LayerMask LoSLayerMask = ((1 << 3) | (1 << 12)); //this is for checking line of sight

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
    }

    private StateMachine<States, StateDriverUnity> fsm;
    // Start is called before the first frame update
    
    // Update is called once per frame
    void Update()
    {

        base.Update();
        fsm.Driver.Update.Invoke();
        currentState = fsm.State;

        if (rb.velocity.x > .3f)
            model.transform.localScale = new Vector3(model.transform.localScale.x, model.transform.localScale.y, -1);
        if (rb.velocity.x < -.3f)
            model.transform.localScale = new Vector3(model.transform.localScale.x, model.transform.localScale.y, 1);

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
        aiPath.enabled = false;
        stateTime = 0;
        Debug.Log("enter idle state");
        aiDestinationSetter.target = this.gameObject.transform;
    }

    void Idle_Update()
    {
        stateTime += Time.deltaTime;
        if (stateTime > idleTime)
            fsm.ChangeState(States.Patrol, StateTransition.Safe);
    }

    void Idle_Exit()
    {
        if (distanceFromPlayer > .05f)
            aiPath.enabled = true;
    }
    
    void Patrol_Enter()
    {
        stateTime = 0;
        WalkToRandomNearbyNode();
        Debug.Log("enter patrol state");   
    }

    
    void Patrol_Update()
    {
        stateTime += Time.deltaTime;
        if (Mathf.Abs(aiPath.velocity.x) < 1f) //this is to allow enemy to basically climb high altitudes
            rb.useGravity = false;
        else
            rb.useGravity = true;
        if (stateTime > patrolTime)
        {
            fsm.ChangeState(States.Idle, StateTransition.Safe);
            rb.useGravity = true;
        }

        UpdatePlayerDirection();
    }

    void Patrol_Exit()
    {
        rb.useGravity = true;
    }

    void Chasing_Enter()
    {
        aiDestinationSetter.target = playerMovement.transform;
    }

    void Chasing_Update()
    {
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceFromPlayer < attackRange) //is enemy close enough to start attack
        {
            fsm.ChangeState(States.AttackWindup, StateTransition.Safe);
        }
        if (Mathf.Abs(aiPath.velocity.x) < 1f && aiPath.velocity.y > .1f) //this is to allow enemy to basically climb high altitudes
            rb.useGravity = false;
        else
            rb.useGravity = true;
        if (stateTime > patrolTime)
        {
            fsm.ChangeState(States.Idle, StateTransition.Safe);
            rb.useGravity = true;
        }

        UpdatePlayerDirection();
        if (playerToTheRight)
            model.transform.localScale = new Vector3(model.transform.localScale.x, model.transform.localScale.y, -1);
        else if (!playerToTheRight)
            model.transform.localScale = new Vector3(model.transform.localScale.x, model.transform.localScale.y, 1);

    }

    void Chasing_Exit()
    {
        aiDestinationSetter.target = null;
        Debug.Log("chasing exit");
    }

    void AttackWindup_Enter()
    {
        attackWindUpSound.Play();
        rb.useGravity = true;
        UpdatePlayerDirection();
        if (playerToTheRight == true)
        {
            rb.AddForce(new Vector2(-1, .5f), ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(new Vector2(1, .5f), ForceMode.Impulse);
        }
        Debug.Log("start attack windup");
        aiPath.enabled = false;
        timer = attackWindup;
    }
    void AttackWindup_Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = 0;
            fsm.ChangeState(States.Attack, StateTransition.Safe);
        }
        UpdatePlayerDirection();
        if (playerToTheRight)
            model.transform.localScale = new Vector3(model.transform.localScale.x, model.transform.localScale.y, -1);
        else if (!playerToTheRight)
            model.transform.localScale = new Vector3(model.transform.localScale.x, model.transform.localScale.y, 1);
    }

    void Attack_Enter()
    {
        attackSound.Play();
        rb.useGravity = true;
        attacking = true;
        timer = 0f;
        timer = attackTime;
        isAttacking = true;
        Debug.Log("start attacK");
        UpdatePlayerDirection();
        if (playerToTheRight == true)
            rb.AddForce(new Vector2(2+distanceFromPlayer, 5+(player.transform.position.y-transform.position.y)), ForceMode.Impulse);
        else
            rb.AddForce(new Vector2(-2-distanceFromPlayer, 5+(player.transform.position.y - transform.position.y)), ForceMode.Impulse);
    }
    void Attack_Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
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
        timer = 0f;
        timer = attackCooldown;
    }
    void AttackCooldown_Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = 0;
            fsm.ChangeState(States.Idle, StateTransition.Safe);
        }
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
        aiDestinationSetter.target = pathingNodes[Random.Range(0, pathingNodes.Count-1)].transform; //set new target to a random nearby node's position
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
        if (collision.gameObject.CompareTag("Player"))
        {
            gameObject.GetComponent<AIPath>().enabled = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameObject.GetComponent<AIPath>().enabled = false;
        }
    }
}
