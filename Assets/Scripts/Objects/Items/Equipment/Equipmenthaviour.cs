using System;
using System.Collections;
using UnityEngine;

public class EquipmentBehaviour : ItemBehaviour
{
    protected EquipmentData equipmentData;

    protected float animalDamage = 5f;
    protected float harvestableDamage = 2f;
    protected float placeableDamage = 5f;

    public override void Initialize(ItemStack stack)
    {
        base.Initialize(stack);
        equipmentData = data as EquipmentData;
    }

    public float GetEquipmentDamage() => equipmentData.damage;

    public override void Attack(ArmController arm)
    {
        if (!canUse)
            return;

        //canUse = false;
        StartCoroutine(UseCooldown());

    }

    public override void Use()
    {
        if (!canUse)
            return;

        //canUse = false;
        StartCoroutine(UseCooldown());
    }

    protected void TakeDamage(float amount)
    {
        if (itemStack == null) return;

        float newHealth = Mathf.Clamp(itemStack.currentHealth - amount, 0, GetMaxHealth()
        );

        itemStack.currentHealth = newHealth;

        if (newHealth <= 0)
        {
            ArmController.instance.ResetArm();

            InventoryController.instance.RemoveItem(HotBarController.instance.GetCurrentItem());
        }

        HotBarController.instance.UpdateEquipmentHealthBar(
            HotBarController.instance.GetSelectedIndex()
        );
    }
}