using System.Collections;
using UnityEditor;
using UnityEngine;

public class Berry : ConsumableBehaviour
{
    private float hungerPoints;
    protected override void Awake()
    {
        base.Awake(); 
        consumeTime = 1;
        hungerPoints = 5f;
    }

    protected override void Consume()
    {
        PlayerController.instance.GetPlayerAttributes().Eat(hungerPoints);
        InventoryController.instance.RemoveItem(HotBarController.instance.GetCurrentItem());
    }
}
