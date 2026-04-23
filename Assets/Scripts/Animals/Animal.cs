using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Video;

public class Animal : MonoBehaviour
{   
    [SerializeField] protected AnimalData data;
    public IAnimalBehaviour animalBehaviour;

    private float currentHealth;

    void Start()
    {
        currentHealth = data.maxHealth;
    }

    public void SetBehaviour(IAnimalBehaviour behaviour) => animalBehaviour = behaviour;

    void Update()
    {
        animalBehaviour?.Act(this);
    }


    public void TakeDamage(float amount)
    {   
        currentHealth = Math.Clamp(currentHealth - amount, 0, data.maxHealth);
        print("Enemigo dañado, vida: " + currentHealth);

        if(currentHealth == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        print("Animal muerto");
    }
}