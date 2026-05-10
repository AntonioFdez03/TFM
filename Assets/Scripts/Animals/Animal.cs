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
        animalBehaviour?.Act(this);
    }
    
    public AnimalData GetAnimalData() => data;


    public void TakeDamage(float amount)
    {   
        currentHealth = Math.Clamp(currentHealth - amount, 0, data.maxHealth);
        print("Animal dañado, vida: " + currentHealth);

        if(currentHealth == 0)
        {
            Die();
        }
        else
        {
            animalBehaviour.TakeDamage(this);
        }
    }

    private void Die()
    {
        print("Animal muerto");
    }
}