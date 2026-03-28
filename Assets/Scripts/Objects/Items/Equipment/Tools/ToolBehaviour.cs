using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public enum ToolType { None, Axe, Pickaxe}
public class ToolBehaviour : EquipmentBehaviour
{
    protected ToolType toolType;

    protected override void Awake()
    {
        base.Awake(); 
    }

    public ToolType GetToolType() => toolType;

    public override void Attack(ArmController arm)
    {
        base.Attack(arm);
        arm.StartCoroutine(arm.ToolSwingCR());
    }

    public override void Use()
    {
        if (!canUse) 
            return;
        
        canUse = false;      
        UseTool();
        StartCoroutine(UseCooldown());
    }

    protected void UseTool()
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

            HarvestableObject harvestableObject = hit.collider.CompareTag("Harvestable") ? hit.collider.GetComponent<HarvestableObject>() : null;
            if(harvestableObject != null)
            {   
                print("Harvestable detectado");
                harvestableObject.TakeHit(toolType,equipmentDamage);
                TakeDamage(harvestableDamage);
            }

            if(hit.collider.TryGetComponent(out PlaceableBehaviour placeable))
            {
                placeable.TakeDamage(equipmentDamage);
                TakeDamage(placeableDamage);
            }
        }
    }
}
