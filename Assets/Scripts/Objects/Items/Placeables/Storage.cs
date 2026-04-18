using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Storage : PlaceableBehaviour, IInteractiveObject
{
    public void Interact()
    {   
        print("Interact");
        UIController.instance.OpenStorage(this);
    }
}