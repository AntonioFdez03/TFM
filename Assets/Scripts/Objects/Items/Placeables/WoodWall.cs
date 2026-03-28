using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WoodWall : PlaceableBehaviour 
{
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