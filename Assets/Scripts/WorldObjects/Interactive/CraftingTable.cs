using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CraftingTable : InteractiveObject
{
    [SerializeField] private Canvas craftingCanvas;

    public override void Interact()
    {
        UIController.instance.SetState(UIState.Crafting);    
    }
}