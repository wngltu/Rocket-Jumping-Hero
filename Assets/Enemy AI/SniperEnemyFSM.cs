using UnityEngine;
using MonsterLove.StateMachine; //use _Enter, _Exit, _Finally for functions after the state enum
using System.Linq;
using Pathfinding;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;

public class SniperEnemyFSM : Enemy
{ 
    public static SniperEnemyFSM GruntInstance;
    float attackWindup = .5f;
    float attackTime = 1f;
    float attackCooldown = 2.5f;
    float attackRange = 15f;
    float baseDamage = 10f;

    float idleTime = 3f;
    float patrolTime = 3f;


    float distanceFromPlayer;
    float timer;

    public bool recoiledRecently = false;

    public GameObject explodeIndicator;
    public GameObject weapon;
    public GameObject weaponModel;
    public GameObject barrel;
    public GameObject barrel2;
    public GameObject bullet;
    public GameObject bulletIndicator;
    public AIDestinationSetter aiDestinationSetter;
    public States currentState;
    public AudioSource shootSound;
    public SpriteRenderer sprite;
    public Sprite idleModel;
    public Sprite attackingModel;
    public Sprite cooldownModel;
    public Sprite windupModel;
    public ParticleSystem thruster;

    LayerMask layerMask = 1 << 3;
    LayerMask LoSLayerMask = ((1 << 3) | (1 << 12)); //this is for checking line of sight

    Vector3 attackDirection;

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
        base.maxHealth = 100;
        base.Start();
        base.weaponLoot = base.prefabLoot.sniperDrop;
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
        base.Update();
        fsm.Driver.Update.Invoke();
        currentState = fsm.State;
        print(currentState);

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
        thruster.Stop();
        sprite.sprite = idleModel;
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
        thruster.Stop();
        sprite.sprite = idleModel;
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
        thruster.Play();
        sprite.sprite = idleModel;
        aiDestinationSetter.target = playerMovement.transform;
    }

    void Chasing_Update()
    {
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceFromPlayer < attackRange) //is enemy close enough to start attack
        {
            fsm.ChangeState(States.AttackWindup, StateTransition.Safe);
        }

        UpdatePlayerDirection();
        if (playerToTheRight)
            model.transform.localScale = new Vector3(-1, model.transform.localScale.y, model.transform.localScale.z);
        else if (!playerToTheRight)
            model.transform.localScale = new Vector3(1, model.transform.localScale.y, model.transform.localScale.z);
    }

    void Chasing_Exit()
    {
        aiDestinationSetter.target = null;
        Debug.Log("chasing exit");
    }

    void AttackWindup_Enter()
    {
        thruster.Stop();
        sprite.sprite = windupModel;
        bulletIndicator.SetActive(true);
        UpdatePlayerDirection();
        weapon.transform.right = player.transform.position - weapon.transform.position;

        barrel.transform.localRotation = Quaternion.LookRotation(GetVectorToPlayer(), Vector3.up);
        barrel2.transform.localRotation = Quaternion.LookRotation(GetVectorToPlayer(), Vector3.up);

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

    void AttackWindup_Exit()
    {
        bulletIndicator.SetActive(false);
    }

    void Attack_Enter()
    {
        sprite.sprite = attackingModel;
        thruster.Stop();
        shootSound.Play();
        timer = 0f;
        timer = attackTime;
        Debug.Log("start attacK");
        UpdatePlayerDirection();
        if (playerToTheRight == true)
        {

            GameObject bulletInstance = Instantiate(bullet, barrel2.transform, false);
            bulletInstance.GetComponent<EnemyBullet>().damage = baseDamage;
            //barrel.transform.localScale = new Vector3(1, 1, 1);
            bulletInstance.transform.SetParent(null);     
        }
        else
        {
            GameObject bulletInstance = Instantiate(bullet, barrel.transform, false);
            bulletInstance.GetComponent<EnemyBullet>().damage = baseDamage;
            //barrel.transform.localScale = new Vector3(-1, 1, 1);
            bulletInstance.transform.SetParent(null);
        }
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
        sprite.sprite = cooldownModel;
        thruster.Stop();
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
