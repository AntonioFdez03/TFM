using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class EquipmentBehaviour : ItemBehaviour
{
    protected float equipmentDamage;
    protected float equipmentRange;

    protected float enemyDamage = 5f;
    protected float harvestableDamage = 2f;
    protected float placeableDamage = 5f;

    public float GetEquipmentDamage() => equipmentDamage;

    protected override void Awake()
    {
        base.Awake(); 
    }

    public override void Attack(ArmController arm)
    {
        base.Attack(arm);
    }

    public override void Use()
    {
        if (!canUse) 
            return;
        
        canUse = false;      
        StartCoroutine(UseCooldown());
    }

    protected void TakeDamage(float amount)
    {   
        currentHealth = Math.Clamp(currentHealth - amount, 0 ,maxHealth);
        print("Vida: " + currentHealth);
        if(currentHealth == 0)
        {
            print("Herramienta destruida");
            InventoryController.instance.RemoveItem(HotBarController.instance.GetCurrentItem());
        }
        
        EquipmentBehaviour originalItem = HotBarController.instance.GetCurrentItem().GetComponent<EquipmentBehaviour>();
        if(originalItem != null)
            originalItem.SetCurrentHealth(currentHealth);
        
        HotBarController.instance.UpdateEquipmentHealthBar(HotBarController.instance.GetSelectedIndex());
    }
}
