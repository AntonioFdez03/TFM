using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Furnace : PlaceableBehaviour, IInteractiveObject
{   
    private FurnaceController furnaceController;

    protected override void Start()
    {
        base.Start();
        furnaceController = GetComponent<FurnaceController>();
    }

    public void Interact()
    {   
        UIController.instance.OpenFurnace(this);
    }

    public FurnaceController GetFurnaceController() => furnaceController;
    public override bool CanUnplace() => furnaceController.IsFurnaceEmpty();
}