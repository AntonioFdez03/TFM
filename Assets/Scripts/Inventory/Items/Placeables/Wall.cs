using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wall : PlaceableBehaviour 
{
    private float currentHealth;
    private float maxHealth;

    protected override void Start()
    {   
        base.Start();

        maxHealth = 100;
        currentHealth = maxHealth;
    }

    void Update()
    {
        
    }
}