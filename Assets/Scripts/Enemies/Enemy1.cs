using Unity.VisualScripting;
using UnityEngine;

public class Enemy1 : Enemy
{  
    override protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        maxHealth = 100;
        currentHealth = maxHealth;
        speed = 7;
        detectionRange = 100;
        damage = 10;
        attackCooldown = 1.2f;
    }

    override protected void Move()
    {
        float playerDistance = Vector3.Distance(transform.position, player.position);

        if(playerDistance <= detectionRange && playerDistance > 5f){
            Vector3 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
        else{
            rb.linearVelocity = Vector3.zero;
        }
    }

    override protected void Attack()
    {
        if(playerAttributes != null)
        {   
            print("Ataca");
            playerAttributes.TakeDamage(10);
        }
    }
}