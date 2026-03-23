using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Workbench : PlaceableBehaviour, IInteractiveObject
{
    [SerializeField] private Canvas craftingCanvas;
    
    protected override void Start()
    {
        base.Start();
        unplaceTime = 1f;
    }
    public void Interact()
    {   
        CraftingController.instance.SetStationType(CraftingStationType.Workbench);
        UIController.instance.SetState(UIState.Crafting);    
    }
}