using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{   
    [SerializeField] protected Transform player;
    protected Rigidbody rb;
    protected float maxHealth;
    protected float currentHealth;
    protected float speed;
    protected int damage;
    protected float detectionRange;
    protected float attackCooldown;
    protected bool canAttack = true;
    protected bool playerInRange = false;
    protected PlayerAttributes playerAttributes;

    protected abstract void Awake();
    protected virtual void Update()
    {
        Move();
        if(playerInRange && canAttack)
        {
            Attack();
            StartCoroutine(AttackCooldown());
        }
    }

    protected abstract void Move();
    protected abstract void Attack();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Jugador detectado");
            playerInRange = true;
            playerAttributes = other.GetComponent<PlayerAttributes>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        print("Jugador perdido");
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void TakeDamage(float playerDamage)
    {
        currentHealth -= playerDamage;
        print("Enemigo recibe daño: " + playerDamage + ", salud restante: " + currentHealth);
        if (currentHealth <= 0)
            Die();
    }

    protected void Die()
    {
        print("Enemigo muerto");
        Destroy(gameObject);
    }
}
