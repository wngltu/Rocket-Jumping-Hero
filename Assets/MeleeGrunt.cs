using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeGrunt : Enemy
{
    public static MeleeGrunt GruntInstance;
    float attackCooldown = 2f;
    float attackCooldownTimer = 0f;
    float attackRange = 3.5f;
    float baseDamage = 10f;

    bool isAttacking = false;
    public bool recoiledRecently = false;

    public GameObject explodeIndicator;

    LayerMask layerMask = 1 << 3;

    Vector3 attackDirection;

    private void Start()
    {
        base.Start();
        GruntInstance = this;
    }
    void Update()
    {
        if (aggroed == false)
            return;

        if (player.gameObject.transform.position.y > transform.position.y) //is player higher altitude than enemy?
        {
            if (Mathf.Abs(aiPath.velocity.x) < 3f) //this is to allow enemy to basically climb high altitudes
                rb.useGravity = false;
            else
                rb.useGravity = true;
        }
        else
            rb.useGravity = true;

        float distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceFromPlayer < 3f && isAttacking == false) //is enemy close enough to start attack
        {
            isAttacking = true;
            StartAttack();
        }
    }

    void StartAttack()
    {
        attackDirection = (player.transform.position - transform.position).normalized;
        RaycastHit hit;
        print("enemy shoot");
        if (Physics.Raycast(transform.position, attackDirection, out hit, attackRange, layerMask)) //shoot ray from barrel of gun
        {
            Debug.DrawRay(transform.position, attackDirection);
            Debug.Log(hit.collider.gameObject.name.ToString());
            hit.collider.gameObject.GetComponentInParent<PlayerHealth>().TakeDamage(baseDamage);
            print(hit.collider.gameObject.name.ToString());

            GameObject newObject = Instantiate(explodeIndicator, hit.point, Quaternion.identity);
            newObject.GetComponent<ExplosiveRadius>().explosionRadius = .35f;
        }
        aiPath.enabled = false;
        Invoke("EndAttackCooldown", attackCooldown);
    }

    void EndAttackCooldown()
    {
        aiPath.enabled = true;
        isAttacking = false;
    }

    void StartRecoiledRecently()
    {
        Invoke("EndRecoiledRecently", 3f);
        rb.useGravity = true;
        GruntInstance.enabled = false;
    }

    void EndRecoiledRecently()
    {
        recoiledRecently = false;
        GruntInstance.enabled = true;
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
