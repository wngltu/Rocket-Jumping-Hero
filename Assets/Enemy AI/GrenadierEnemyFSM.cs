using UnityEngine;
using MonsterLove.StateMachine; //use _Enter, _Exit, _Finally for functions after the state enum
using System.Linq;
using Pathfinding;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;

public class GrenadierEnemyFSM : Enemy
{ 
    public static GrenadierEnemyFSM GruntInstance;
    float attackWindup = .75f;
    float attackTime = 1f;
    float attackCooldown = 3f;
    float attackRange = 7.5f;
    float baseDamage = 10f;

    float idleTime = 1f;
    float patrolTime = 2f;


    float distanceFromPlayer;
    float timer;

    public bool recoiledRecently = false;

    public GameObject explodeIndicator;
    public GameObject weaponModel;
    public GameObject barrel;
    public GameObject bullet;
    public AIDestinationSetter aiDestinationSetter;
    public States currentState;

    LayerMask layerMask = 1 << 3;

    Vector3 attackDirection;

    public enum States
    {
        Init,
        Idle,
        Patrol,
        Attack,
        AttackCooldown,
        Chasing,
        AttackWindup
    }
    float aggroRange = 20f;
    float stateTime = 0f;

    StateMachineRunner stateMachineRunner;
    PlayerMovement playerMovement;

    private void Start()
    {
        fsm = new StateMachine<States, StateDriverUnity>(this);
        fsm.ChangeState(States.Init, StateTransition.Safe);
        base.maxHealth = 175;
        base.Start();
        base.weaponLoot = base.prefabLoot.grenadeDrop;
        GruntInstance = this;

        if (aiDestinationSetter == null)
            aiDestinationSetter = GetComponent<AIDestinationSetter>();
        if (playerMovement == null)
            playerMovement = FindObjectOfType<PlayerMovement>();
        if (stateMachineRunner == null)
            stateMachineRunner = FindObjectOfType<StateMachineRunner>();
    }

    private StateMachine<States, StateDriverUnity> fsm;

    void Update()
    {
        fsm.Driver.Update.Invoke();
        currentState = fsm.State;
        print(currentState);
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
        Vector3 relativePos = player.transform.position - transform.position;

        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        weaponModel.transform.rotation = rotation;
    }

    void Chasing_Exit()
    {
        aiDestinationSetter.target = null;
        Debug.Log("chasing exit");
    }

    void AttackWindup_Enter()
    {
        UpdatePlayerDirection();
        
        if (playerToTheRight == true)
        {
            weaponModel.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            weaponModel.transform.localScale = new Vector3(-1, 1, 1);
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
    }

    void Attack_Enter()
    {
        timer = 0f;
        timer = attackTime;
        Debug.Log("start attacK");
        UpdatePlayerDirection();
        GameObject grenadeInstance = Instantiate(bullet, barrel.transform, false);
        UpdatePlayerDirection();
        if (playerToTheRight == true)
            grenadeInstance.GetComponent<Rigidbody>().AddForce(new Vector2(distanceFromPlayer/3, 2 +(player.transform.position.y - transform.position.y)), ForceMode.Impulse);
        else
            grenadeInstance.GetComponent<Rigidbody>().AddForce(new Vector2(-distanceFromPlayer/3, 2 +(player.transform.position.y - transform.position.y)), ForceMode.Impulse);
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
        if (other.gameObject.CompareTag("Player"))
        {
            aggroed = true;
            fsm.ChangeState(States.Chasing, StateTransition.Safe);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && fsm.State == States.Idle)
        {
            aggroed = true;
            fsm.ChangeState(States.Chasing, StateTransition.Safe);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            aggroed = false;
            fsm.ChangeState(States.Idle, StateTransition.Safe);
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

}
