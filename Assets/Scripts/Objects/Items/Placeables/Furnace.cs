using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Furnace : PlaceableBehaviour, IInteractiveObject
{
    public void Interact()
    {   
        UIController.instance.OpenFurnace(this);
    }
}