using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Workbench : InteractiveObject
{
    [SerializeField] private Canvas craftingCanvas;

    public override void Interact()
    {
        UIController.instance.SetState(UIState.Crafting);    
        CraftingController.instance.SetStationType(CraftingStationType.Workbench);
    }
}