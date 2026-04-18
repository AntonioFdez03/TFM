using System.Collections;
using UnityEditor;
using UnityEngine;

public class Berry : ConsumableBehaviour
{
    protected override void Consume()
    {
        PlayerController.instance.GetPlayerAttributes().Eat(consumableData.hungerPoints);
        InventoryController.instance.RemoveItem(HotBarController.instance.GetCurrentItem());
    }
}
