using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CraftingTable : InteractiveObject
{
    [SerializeField] private Canvas craftingInterface;

    protected void Start()
    {
    }

    public override void Interact()
    {
        print("Se muestra la interfaz");
        UIController.SetState(UIState.Crafting);
    }
}