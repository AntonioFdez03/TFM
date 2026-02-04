using Unity.VisualScripting;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{   
    [SerializeField] protected Transform player;
    protected Rigidbody rb;
    protected float maxHealth;
    protected float currentHealth;
    protected float speed;
    protected float attackRange;
    protected bool canAttack;


    protected abstract void Awake();
    protected virtual void Update()
    {
            Attack();
    }

    protected abstract void Attack();
}
