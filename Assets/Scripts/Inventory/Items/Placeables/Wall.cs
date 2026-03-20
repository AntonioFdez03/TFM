using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wall : PlaceableBehaviour 
{
    private float currentHealth;
    private float maxHealth;

    void Start()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
    }

    void Update()
    {
        
    }
}