using UnityEngine;
using MonsterLove.StateMachine; //use _Enter, _Exit, _Finally for functions after the state enum
using System.Collections;
using System.Linq;
using Pathfinding;
using System.Collections.Generic;
using TMPro;

public class MeleeGruntFSM : Enemy
{
    public static MeleeGruntFSM GruntInstance;
    float attackCooldown = 2f;
    float attackCooldownTimer = 0f;
    float attackRange = 3.5f;
    float baseDamage = 10f;

    bool isAttacking = false;
    public bool recoiledRecently = false;

    public GameObject explodeIndicator;
    public AIDestinationSetter aiDestinationSetter;

    LayerMask layerMask = 1 << 3;

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
        Chasing
    }
    bool aggroed = false;
    float aggroRange = 20f;
    float stateTime = 0f;

    StateMachineRunner stateMachineRunner;
    PlayerMovement playerMovement;
    AIPath aiPath;
    Rigidbody rb;

    private void Start()
    {
        fsm = new StateMachine<States, StateDriverUnity>(this);
        fsm.ChangeState(States.Init);

        base.Start();
        GruntInstance = this;

        if (aiDestinationSetter == null)
            aiDestinationSetter = GetComponent<AIDestinationSetter>();
        if (playerMovement == null)
            playerMovement = FindObjectOfType<PlayerMovement>();
        if (aiPath == null)
            aiPath = GetComponent<AIPath>();
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        if (stateMachineRunner == null)
            stateMachineRunner = FindObjectOfType<StateMachineRunner>();
    }

    private StateMachine<States, StateDriverUnity> fsm;
    // Start is called before the first frame update
    
    // Update is called once per frame
    void Update()
    {
        fsm.Driver.Update.Invoke();
    }

    void Init_Enter()
    {
        Debug.Log("enter init state");
        fsm.ChangeState(States.Patrol);
    }
    void Init_Exit()
    {
        Debug.Log("exit init state");
    }
    
    void Idle_Enter()
    {
        stateTime = 0;
        Debug.Log("enter idle state");
        aiDestinationSetter.target = this.gameObject.transform;
    }

    void Idle_Update()
    {
        stateTime += Time.deltaTime;
        Debug.Log("swaws");
        if (stateTime > 3)
            fsm.ChangeState(States.Patrol);
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
        Debug.Log("swaws 2");
        if (Mathf.Abs(aiPath.velocity.y) > .5f) //this is to allow enemy to basically climb high altitudes
            rb.useGravity = false;
        else
            rb.useGravity = true;
        if (stateTime > 5) 
            fsm.ChangeState(States.Idle);
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
        Debug.Log("swaws 3");
    }

    void Chasing_Exit()
    {
        aiDestinationSetter.target = null;
        Debug.Log("chasing exit");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            aggroed = true;
            fsm.ChangeState(States.Chasing);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            aggroed = false;
            fsm.ChangeState(States.Idle);
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

    override protected void Die()
    {
        if (Died != true)
        {
            GameObject loot = Instantiate(prefabLoot.pistolDrop, this.transform);
            loot.gameObject.transform.SetParent(null);
            Died = true;
            Destroy(this.gameObject);
        }
    }
}
