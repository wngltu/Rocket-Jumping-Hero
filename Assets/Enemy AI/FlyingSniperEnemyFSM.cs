using UnityEngine;
using MonsterLove.StateMachine; //use _Enter, _Exit, _Finally for functions after the state enum
using System.Linq;
using Pathfinding;
using System.Collections.Generic;

public class FlyingSniperEnemyFSM : Enemy
{
    public static FlyingSniperEnemyFSM GruntInstance;
    float attackWindup = .75f;
    float attack2Windup = .5f;
    float attackTime = 2f;
    float attackCooldown = 3f;
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
    public GameObject bulletIndicator;
    public GameObject weapon;
    public GameObject weaponModel;
    public GameObject barrel;
    public GameObject bullet;
    public AudioSource shootSound;

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
        Attack2,
        AttackCooldown,
        Chasing,
        AttackWindup,
        Attack2Windup,
        AttackBridge //bridge between attack 1 and attack 2 
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
    }

    void Chasing_Enter()
    {
        aiDestinationSetter.target = playerMovement.transform;
    }

    void Chasing_Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, hoverDist, HoverLayerMask) && rb.velocity.y < hoverVeloCap)
        {
            rb.velocity += new Vector3(0, .1f, 0);
        }
        else if (rb.velocity.y > -hoverVeloCap)
            rb.velocity -= new Vector3(0, .1f, 0);
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceFromPlayer < attackRange) //is enemy close enough to start attack
        {
            fsm.ChangeState(States.AttackWindup, StateTransition.Safe);
        }
        if (stateTime > patrolTime)
        {
            fsm.ChangeState(States.Idle, StateTransition.Safe);
        }
    }

    void Chasing_Exit()
    {
        aiDestinationSetter.target = null;
        Debug.Log("chasing exit");
    }

    void AttackWindup_Enter()
    {
        bulletIndicator.SetActive(true);
        UpdatePlayerDirection();
        weapon.transform.right = player.transform.position - weapon.transform.position;
        if (playerToTheRight == true)
        {
            weaponModel.transform.localScale = new Vector3(1, 1, 1);
            barrel.transform.localRotation = Quaternion.Euler(90, -90, 0);
        }
        else
        {
            weaponModel.transform.localScale = new Vector3(1, -1, 1);
            barrel.transform.localRotation = Quaternion.Euler(-90, 0, 90);
        }


        Debug.Log("start attack windup");
        aiPath.enabled = false;
        timer = attackWindup;
    }
    void AttackWindup_Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, hoverDist, HoverLayerMask) && rb.velocity.y < hoverVeloCap)
        {
            rb.velocity += new Vector3(0, .1f, 0);
        }
        else if (rb.velocity.y > -hoverVeloCap)
            rb.velocity -= new Vector3(0, .1f, 0);

        if (rb.velocity.x > 0)
            rb.velocity -= new Vector3(.025f, 0, 0);
        else if (rb.velocity.x < 0)
            rb.velocity += new Vector3(.025f, 0, 0);

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = 0;
            fsm.ChangeState(States.Attack, StateTransition.Safe);
        }
    }

    void AttackWindup_Exit()
    {
        bulletIndicator.SetActive(false);
    }

    void Attack_Enter()
    {
        shootSound.Play();
        timer = 0f;
        timer = attackTime;
        Debug.Log("start attacK");
        UpdatePlayerDirection();
        if (playerToTheRight == true)
        {

            GameObject bulletInstance = Instantiate(bullet, barrel.transform, false);
            bulletInstance.GetComponent<EnemyBullet>().damage = baseDamage;
            bulletInstance.GetComponent<Rigidbody>().AddForce(weapon.transform.right * 500f);
            bulletInstance.transform.SetParent(null);
        }
        else
        {
            GameObject bulletInstance = Instantiate(bullet, barrel.transform, false);
            bulletInstance.GetComponent<EnemyBullet>().damage = baseDamage;
            bulletInstance.GetComponent<Rigidbody>().AddForce(weapon.transform.right * 500f);
            bulletInstance.transform.SetParent(null);
        }
    }
    void Attack_Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, hoverDist, HoverLayerMask) && rb.velocity.y < hoverVeloCap)
        {
            rb.velocity += new Vector3(0, .1f, 0);
        }
        else if (rb.velocity.y > -hoverVeloCap)
            rb.velocity -= new Vector3(0, .1f, 0);
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = 0;
            fsm.ChangeState(States.AttackBridge, StateTransition.Safe);
        }
    }

    void AttackBridge_Enter()
    {
        timer = 2.5f;
        if (playerToTheRight)
            rb.AddForce(150, 0, 0);
        else if (!playerToTheRight)
            rb.AddForce(-150, 0, 0);
    }
    void AttackBridge_Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, hoverDist, HoverLayerMask) && rb.velocity.y < hoverVeloCap)
        {
            rb.velocity += new Vector3(0, .1f, 0);
        }
        else if (rb.velocity.y > -hoverVeloCap)
            rb.velocity -= new Vector3(0, .1f, 0);

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = 0;
            fsm.ChangeState(States.Attack2Windup, StateTransition.Safe);
        }
    }

    void Attack2Windup_Enter()
    {
        bulletIndicator.SetActive(true);
        UpdatePlayerDirection();
        weapon.transform.right = player.transform.position - weapon.transform.position;
        if (playerToTheRight == true)
        {
            weaponModel.transform.localScale = new Vector3(1, 1, 1);
            barrel.transform.localRotation = Quaternion.Euler(90, -90, 0);
        }
        else
        {
            weaponModel.transform.localScale = new Vector3(1, -1, 1);
            barrel.transform.localRotation = Quaternion.Euler(-90, 0, 90);
        }


        Debug.Log("start attack windup");
        aiPath.enabled = false;
        timer = attackWindup;
    }
    void Attack2Windup_Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, hoverDist, HoverLayerMask) && rb.velocity.y < hoverVeloCap)
        {
            rb.velocity += new Vector3(0, .1f, 0);
        }
        else if (rb.velocity.y > -hoverVeloCap)
            rb.velocity -= new Vector3(0, .1f, 0);

        if (rb.velocity.x > 0)
            rb.velocity -= new Vector3(.025f, 0, 0);
        else if (rb.velocity.x < 0)
            rb.velocity += new Vector3(.025f, 0, 0);

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = 0;
            fsm.ChangeState(States.Attack2, StateTransition.Safe);
        }
    }

    void Attack2Windup_Exit()
    {
        bulletIndicator.SetActive(false);
    }

    void Attack2_Enter()
    {
        shootSound.Play();
        timer = 0f;
        timer = attackTime;
        Debug.Log("start attacK");
        UpdatePlayerDirection();
        if (playerToTheRight == true)
        {

            GameObject bulletInstance = Instantiate(bullet, barrel.transform, false);
            bulletInstance.GetComponent<EnemyBullet>().damage = baseDamage;
            bulletInstance.GetComponent<Rigidbody>().AddForce(weapon.transform.right * 500f);
            bulletInstance.transform.SetParent(null);
        }
        else
        {
            GameObject bulletInstance = Instantiate(bullet, barrel.transform, false);
            bulletInstance.GetComponent<EnemyBullet>().damage = baseDamage;
            bulletInstance.GetComponent<Rigidbody>().AddForce(weapon.transform.right * 500f);
            bulletInstance.transform.SetParent(null);
        }
        fsm.ChangeState(States.AttackCooldown, StateTransition.Safe);
    }

    void AttackCooldown_Enter()
    {
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
}