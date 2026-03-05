using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Workbench : InteractiveObject
{
    [SerializeField] private Canvas craftingCanvas;

    public override void Interact()
    {
        CraftingController.instance.SetStationType(CraftingStationType.Workbench);
        UIController.instance.SetState(UIState.Crafting);    
    }
}