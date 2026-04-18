using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Workbench : PlaceableBehaviour, IInteractiveObject
{
    public void Interact()
    {   
        CraftingController.instance.SetStationType(CraftingStationType.Workbench);
        UIController.instance.SetState(UIState.Crafting);    
    }
}