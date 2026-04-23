using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Storage : PlaceableBehaviour, IInteractiveObject
{
    private StorageController storageController;

    protected override void Start()
    {
        base.Start();
        storageController = GetComponent<StorageController>();
    }

    public void Interact()
    {  
        UIController.instance.OpenStorage(this);
    }

    public override bool CanUnplace()
    {
        if(storageController.GetItems().Length > 0)
            return false;

        return true;
    }
}