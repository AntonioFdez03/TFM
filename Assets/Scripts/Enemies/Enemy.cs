using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Video;

public abstract class Enemy : MonoBehaviour
{   
    protected NavMeshAgent agent;
    [SerializeField] protected Transform player;
    protected Rigidbody rb;
    protected float maxHealth;
    protected float currentHealth;
    protected float speed;
    protected int damage;
    protected float detectionRange;
    protected float attackRange;
    protected float attackCooldown;
    protected bool playerInRange = false;
    protected bool canAttack = true;
    protected float flinchCooldown;
    protected float KnockbackForce;

    protected abstract void Awake();
    protected virtual void Update()
    {
    
        if(agent.isActiveAndEnabled)
            Move();
        if(playerInRange && canAttack)
        {
            Attack();
            StartCoroutine(AttackCooldownCR());
        }
    }

    protected abstract void Move();
    protected abstract void Attack();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        print("Jugador perdido");
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    public void TakeDamage(float playerDamage)
    {
        currentHealth = Math.Clamp(currentHealth - playerDamage, 0 ,maxHealth);
        if (currentHealth == 0)
            Die();

        print("Enemigo recibe daño: " + playerDamage + ", salud restante: " + currentHealth);
        StartCoroutine(FlinchCooldownCR());
    }


    protected void Die()
    {
        Destroy(gameObject);
    }

    protected IEnumerator AttackCooldownCR()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    protected IEnumerator FlinchCooldownCR()
    {
        agent.enabled = false;
        Vector3 direction = (transform.position - player.position).normalized;

        rb.linearVelocity = Vector3.zero; // limpia velocidad previa
        rb.AddForce(direction * KnockbackForce, ForceMode.Impulse);
        yield return new WaitForSeconds(flinchCooldown);
        agent.enabled = true;
    }


}
