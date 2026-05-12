using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Video;

public class Animal : MonoBehaviour
{   
    [SerializeField] protected AnimalData data;
    private IAnimalBehaviour animalBehaviour;
    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;

    private bool dead = false;


    private float currentHealth;

    void Start()
    {
        currentHealth = data.maxHealth;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    public Transform GetPlayer() => player;
    public void SetPlayer(Transform p) => player = p; 
    public void SetBehaviour(IAnimalBehaviour behaviour) => animalBehaviour = behaviour;
    public Animator GetAnimator() => animator;
    public NavMeshAgent GetAgent() => agent;

    void Update()
    {   
        if(!dead)
            animalBehaviour?.Act(this);
    }
    
    public AnimalData GetAnimalData() => data;


    public void TakeDamage(float amount)
    {   
        currentHealth = Math.Clamp(currentHealth - amount, 0, data.maxHealth);

        print("Vida restante: " + currentHealth);
        if(currentHealth == 0)
        {
            Die();
        }
        else
        {
            animalBehaviour.TakeDamage(this);
        }
    }

    public IEnumerator KnockbackCR(Vector3 dir, float force, float duration)
    {
        NavMeshAgent agent = GetAgent();

        agent.isStopped = true;   // importante
        agent.updatePosition = true;

        Vector3 start = transform.position;
        Vector3 target = start + dir * force;

        float t = 0f;

        while (t < duration)
        {
            float step = t / duration;

            Vector3 pos = Vector3.Lerp(start, target, step);

            agent.Move(pos - transform.position); // 👈 clave (NO transform.position)

            t += Time.deltaTime;
            yield return null;
        }

        agent.Warp(target); // re-sincroniza sin salto visible
        agent.isStopped = false;
    }

    private void Die()
    {
        dead = true;
        animator.SetTrigger("Dead");
        player.GetComponent<PlayerController>().GetPlayerAttributes().UpdateSanity(-10f);
    }
}