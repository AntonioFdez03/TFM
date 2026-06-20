using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class WeaponBehaviour : EquipmentBehaviour
{
    protected WeaponData weaponData;

    public override void Initialize(ItemStack stack)
    {
        base.Initialize(stack);
        weaponData = equipmentData as WeaponData;
    }
    
    public override void Attack(ArmController arm)
    {
        base.Attack(arm);
    }

    public override void Use()
    {
        if (!canUse) 
            return;
        
        //canUse = false;      
        UseWeapon();
        StartCoroutine(UseCooldown());
    }

    protected void UseWeapon()
    {
        Ray ray = new Ray(CameraController.instance.transform.position, CameraController.instance.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * weaponData.range, Color.red);
        if (Physics.Raycast(ray, out hit, weaponData.range))
            ApplyDamage(hit.collider.gameObject, weaponData.damage);
        
    }

    protected void ApplyDamage(GameObject target, float damage)
    {   
        Transform parent = target.transform.parent;
        if(parent != null && parent.TryGetComponent(out Animal animal))
        {
            animal.TakeDamage(damage);
            TakeDamage(animalDamage);
        }

        if(target.TryGetComponent(out PlaceableBehaviour placeable))
        {
            placeable.TakeDamage(damage/10);
            TakeDamage(placeableDamage);
        }
    }
}
