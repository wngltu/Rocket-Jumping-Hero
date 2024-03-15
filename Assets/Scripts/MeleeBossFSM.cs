using UnityEngine;
using MonsterLove.StateMachine; //use _Enter, _Exit, _Finally for functions after the state enum
using System.Linq;
using Pathfinding;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;

public class MeleeBossFSM : Enemy
{
    public static MeleeBossFSM GruntInstance;
    float attackWindup = .75f;
    float attackTime = 1.5f;
    float attackCooldown = 2f;
    float attackRange = 3.5f;
    float baseDamage = 50f;

    float idleTime = 3f;
    float patrolTime = 5f;

    public float shieldCharges = 0f;

    float distanceFromPlayer;
    float timer;

    public bool recoiledRecently = false;
    bool isAttacking = true; //this is to make sure the enemy does not hit the player more than once in one attack

    public GameObject explosionObject;
    public GameObject explodeIndicator;
    public GameObject leftBarrel;
    public GameObject rightBarrel;
    public GameObject slamProjectile;
    public GameObject invincibleShield;
    public AIDestinationSetter aiDestinationSetter;
    public States currentState;
    public AudioSource attackWindUpSound;
    public AudioSource attackSound;

    LayerMask LoSLayerMask = ((1 << 3) | (1 << 12)); //this is for checking line of sight

    Vector3 attackDirection;

    public enum States
    {
        Init,
        Idle,
        Patrol,
        Chasing,

        Attack,
        AttackCooldown,
        AttackWindup,

        SlamAttack,
        SlamAttackCooldown,
        SlamAttackWindup,

        Died
    }
    float aggroRange = 20f;
    float stateTime = 0f;

    StateMachineRunner stateMachineRunner;
    PlayerMovement playerMovement;

    private void Start()
    {
        fsm = new StateMachine<States, StateDriverUnity>(this);
        fsm.ChangeState(States.Init, StateTransition.Safe);

        maxHealth = 250f;
        base.Start();
        base.weaponLoot = base.prefabLoot.rifleDrop;
        GruntInstance = this;
        isBoss = true;

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
        if (shieldCharges == 0)
        {
            isInvincible = false;
        }
        if (shieldCharges > 0)
        {
            isInvincible = true;
        }
        if (isInvincible && invincibleShield.activeSelf == false)
            invincibleShield.SetActive(true);
        else if (!isInvincible && invincibleShield.activeSelf == true)
            invincibleShield.SetActive(false);
        base.Update();
        fsm.Driver.Update.Invoke();
        currentState = fsm.State;

        if (Died)
            fsm.ChangeState(States.Died, StateTransition.Safe);
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
        timer = 0;
    }

    void Chasing_Update()
    {
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceFromPlayer < attackRange) //is enemy close enough to start attack
        {
            switch (Random.Range(0,2))
            {
            case 0:
                    fsm.ChangeState(States.SlamAttackWindup, StateTransition.Safe);
                break;
            case 1:
                    fsm.ChangeState(States.AttackWindup, StateTransition.Safe);
                break;
            }
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

        if (distanceFromPlayer < attackRange*4)
            timer += Time.deltaTime;
        if (timer > 2) //every 2 seconds, check rng to do slam attack or not
        {
            if (Random.Range(0, 2) == 0)
            {
                fsm.ChangeState(States.SlamAttackWindup);
            }
        }

        UpdatePlayerDirection();
        if (playerToTheRight)
            model.transform.localScale = new Vector3(1, model.transform.localScale.y, model.transform.localScale.z);
        else if (!playerToTheRight)
            model.transform.localScale = new Vector3(-1, model.transform.localScale.y, model.transform.localScale.z);
    }

    void Chasing_Exit()
    {
        aiDestinationSetter.target = null;
        Debug.Log("chasing exit");
        timer = 0;
    }

    void AttackWindup_Enter()
    {
        attackWindUpSound.Play();
        rb.useGravity = true;
        UpdatePlayerDirection();
        if (playerToTheRight == true)
            rb.AddForce(new Vector2(-1, .5f), ForceMode.Impulse);
        else
            rb.AddForce(new Vector2(1, .5f), ForceMode.Impulse);
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
        isInvincible = false;
        attackSound.Play();
        rb.useGravity = true;
        attacking = true;
        timer = 0f;
        timer = attackTime;
        isAttacking = true;
        Debug.Log("start attacK");
        UpdatePlayerDirection();
        if (playerToTheRight == true)
            rb.AddForce(new Vector2(2 + distanceFromPlayer, 5 + (player.transform.position.y - transform.position.y)), ForceMode.Impulse);
        else
            rb.AddForce(new Vector2(-2 - distanceFromPlayer, 5 + (player.transform.position.y - transform.position.y)), ForceMode.Impulse);

        UpdatePlayerDirection();
        if (playerToTheRight)
            model.transform.localScale = new Vector3(1, model.transform.localScale.y, model.transform.localScale.z);
        else if (!playerToTheRight)
            model.transform.localScale = new Vector3(-1, model.transform.localScale.y, model.transform.localScale.z);
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
        isInvincible = false;
        shieldCharges = 0;
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
    void AttackCooldown_Exit()
    {
        isInvincible = true;
        shieldCharges = 1;
    }

    void SlamAttackWindup_Enter()
    {
        attackWindUpSound.Play();
        rb.useGravity = true;
        Debug.Log("start attack windup");
        aiPath.enabled = false;
        timer = attackWindup;
    }
    void SlamAttackWindup_Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = 0;
            fsm.ChangeState(States.SlamAttack, StateTransition.Safe);
        }
    }

    bool slamAttacking = false;
    bool slamAttackLand = false; //when the enemy touches the ground, explode in projectiles?
    void SlamAttack_Enter()
    {
        slamAttacking = true;
        slamAttackLand = false;
        attackSound.Play();
        rb.useGravity = true;
        attacking = true;
        timer = 0f;
        timer = attackTime*2;
        isAttacking = true;
        Debug.Log("start attacK");
        UpdatePlayerDirection();
        rb.AddForce(new Vector3(0, 5, 0), ForceMode.Impulse);
    }
    void SlamAttack_Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = 0;
            fsm.ChangeState(States.SlamAttackCooldown, StateTransition.Safe);
        }

        if (slamAttackLand == true)
        {
            GameObject obj = Instantiate(explosionObject, this.transform);
            obj.GetComponent<ExplosionScript>().EnemyExplode(baseDamage, 5, 30f);
            Instantiate(slamProjectile, leftBarrel.transform);
            Instantiate(slamProjectile, rightBarrel.transform);
            fsm.ChangeState(States.SlamAttackCooldown, StateTransition.Safe);
        }
    }


    void SlamAttackCooldown_Enter()
    {
        isInvincible = false;
        shieldCharges = 0;
        timer = 0f;
        timer = attackCooldown * 1.5f;
    }
    void SlamAttackCooldown_Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = 0;
            fsm.ChangeState(States.Idle, StateTransition.Safe);
        }
    }
    void SlamAttackCooldown_Exit()
    {
        isInvincible = true;
        shieldCharges = 1;
    }

    void Died_Enter()
    {
        isAttacking = false;
    }

    public override void takeDamage(float damage)
    {
        if (isInvincible && shieldCharges > 0)
        {
            damage = 0;
        }
        GameObject damageNumber = Instantiate(damageIndicator, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);
        damageNumber.GetComponentInChildren<DamageNumberScript>().damage = damage;
        if (damage > 0)
        {
            currentHealth -= damage;
        }
        healthBar.value = currentHealth / maxHealth;
        if (currentHealth <= 0f)
        {
            Die();
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isAttacking == true)
        {
            isAttacking = false;
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(25);
            print(collision.gameObject.name);
            fsm.ChangeState(States.AttackCooldown, StateTransition.Safe);
        }
        if ((collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("platform") || collision.gameObject.CompareTag("ground")) && slamAttacking)
        {
            slamAttackLand = true;
        }
    }
}