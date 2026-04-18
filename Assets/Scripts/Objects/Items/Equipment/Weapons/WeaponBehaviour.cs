using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class WeaponBehaviour : EquipmentBehaviour
{
    protected WeaponData weaponData;

    void Start()
    {
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
        {
            Enemy enemy = hit.collider.CompareTag("Enemy") ? hit.collider.GetComponent<Enemy>() : null;
            if (enemy != null)
            {
                enemy.TakeDamage(weaponData.damage);
                TakeDamage(enemyDamage);
            }

            if(hit.collider.TryGetComponent(out PlaceableBehaviour placeable))
            {
                placeable.TakeDamage(weaponData.damage/10);
                TakeDamage(placeableDamage);
            }
        }
    }
}
