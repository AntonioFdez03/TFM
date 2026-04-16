using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Furnace : PlaceableBehaviour, IInteractiveObject
{
    protected override void Start()
    {
        base.Start();
        maxHealth = 70;
        currentHealth = maxHealth;
    }
    public void Interact()
    {   
        UIController.instance.OpenFurnace(this);
    }
}