using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public enum ToolType { None, Axe, Pickaxe, Hammer}
public class ToolBehaviour : EquipmentBehaviour
{   
    protected ToolData toolData;

    public ToolType GetToolType() => toolData.toolType;

    public override void Initialize(ItemStack stack)
    {
        base.Initialize(stack);
        toolData = equipmentData as ToolData;
    }

    public override void Attack(ArmController arm)
    {
        base.Attack(arm);
        if(toolData.toolType == ToolType.Pickaxe || toolData.toolType == ToolType.None || toolData.toolType == ToolType.Hammer)
            arm.StartCoroutine(arm.PickaxeSwingCR());
        else if(toolData.toolType == ToolType.Axe)
            arm.StartCoroutine(arm.AxeSwingCR());
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

        Debug.DrawRay(ray.origin, ray.direction * toolData.range, Color.red);
        if (Physics.Raycast(ray, out hit, toolData.range))
        {
            Animal animal = hit.collider.CompareTag("Animal") ? hit.collider.GetComponentInParent<Animal>() : null;
            if (animal != null)
            {
                animal.TakeDamage(toolData.damage);
                TakeDamage(animalDamage);
            }

            if(hit.collider.TryGetComponent(out HarvestableObject harvestableObject)){
                harvestableObject.TakeHit(toolData.toolType,toolData.damage);
                TakeDamage(harvestableDamage);
            }

            if(hit.collider.TryGetComponent(out PlaceableBehaviour placeable))
            {
                if(toolData.toolType == ToolType.Hammer)
                {
                    if(placeable.GetCurrentHealth() < placeable.GetMaxHealth())
                    {
                        placeable.Heal(toolData.damage);
                        TakeDamage(placeableDamage);
                    }
                }
                else
                {
                    placeable.TakeDamage(toolData.damage);
                    TakeDamage(placeableDamage);
                }
            }
        }
    }
}
