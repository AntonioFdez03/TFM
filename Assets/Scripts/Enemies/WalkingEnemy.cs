using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Diagnostics;

public class WalkingEnemy : Enemy
{  
    override protected void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        maxHealth = 100;
        currentHealth = maxHealth;
        speed = 5;
        detectionRange = 100;
        damage = 10;
        attackCooldown = 1.2f;
        flinchCooldown = 1f;
        KnockbackForce = 3f;
        attackRange = 5f;
        agent.stoppingDistance = attackRange;
    }

    override protected void Move()
    {   
        float playerDistance = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
        if(playerDistance < detectionRange && playerDistance > 5f){
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else{
            agent.isStopped = true;
        }
    }

    override protected void Attack()
    {
        PlayerAttributes playerAttributes = target.GetComponent<PlayerAttributes>();
        if(playerAttributes != null)
        {  
            playerAttributes.TakeDamage(10);
        }
    }
}