using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class WeaponBehaviour : EquipmentBehaviour
{
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
        UseWeapon();
        StartCoroutine(UseCooldown());
    }

    protected void UseWeapon()
    {
        Ray ray = new Ray(CameraController.instance.transform.position, CameraController.instance.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * equipmentRange, Color.red);
        if (Physics.Raycast(ray, out hit, equipmentRange))
        {
            Enemy enemy = hit.collider.CompareTag("Enemy") ? hit.collider.GetComponent<Enemy>() : null;
            if (enemy != null)
            {
                enemy.TakeDamage(equipmentDamage);
                TakeDamage(enemyDamage);
            }

            if(hit.collider.TryGetComponent(out PlaceableBehaviour placeable))
            {
                placeable.TakeDamage(equipmentDamage/10);
                TakeDamage(placeableDamage);
            }
        }
    }
}
