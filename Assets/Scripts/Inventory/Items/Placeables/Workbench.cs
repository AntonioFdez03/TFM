using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Workbench : PlaceableBehaviour, IInteractiveObject
{
    protected override void Start()
    {
        base.Start();
        maxHealth = 50;
    }
    public void Interact()
    {   
        CraftingController.instance.SetStationType(CraftingStationType.Workbench);
        UIController.instance.SetState(UIState.Crafting);    
    }
}