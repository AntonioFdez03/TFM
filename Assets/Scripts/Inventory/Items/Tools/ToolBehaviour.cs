using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public enum ToolType { None, Axe, Pickaxe}
public class ToolBehaviour : ItemBehaviour
{
    protected ToolType toolType;
    protected float toolDamage;
    protected float toolRange;

    protected float maxhealth;
    protected float currentHealth;

    public ToolType GetToolType() => toolType;
    public float GetToolDamage() => toolDamage;
    public float GetToolMaxHealth() => maxhealth;
    public float GetToolCurrentHealth() => currentHealth;
    public void SetToolCurrentHealth(float health) => currentHealth = health;

    protected override void Awake()
    {
        base.Awake(); 
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

        Debug.DrawRay(ray.origin, ray.direction * toolRange, Color.red);
        if (Physics.Raycast(ray, out hit, toolRange))
        {
            Enemy enemy = hit.collider.CompareTag("Enemy") ? hit.collider.GetComponent<Enemy>() : null;
            if (enemy != null)
            {
                enemy.TakeDamage(toolDamage);
                TakeDamage(5);
            }

            HarvestableObject harvestableObject = hit.collider.CompareTag("Harvestable") ? hit.collider.GetComponent<HarvestableObject>() : null;
            if(harvestableObject != null)
            {
                harvestableObject.TakeHit(toolType,toolDamage);
                TakeDamage(2f);
            }
        }
    }

    protected void TakeDamage(float amount)
    {   
        currentHealth = Math.Clamp(currentHealth - amount, 0 ,maxhealth);
        print("Vida: " + currentHealth);
        if(currentHealth == 0)
        {
            print("Herramienta destruida");
            InventoryController.instance.RemoveItem(HotBarController.instance.GetCurrentItem());
        }
        
        ToolBehaviour originalItem = HotBarController.instance.GetCurrentItem().GetComponent<ToolBehaviour>();
        if(originalItem != null)
            originalItem.SetToolCurrentHealth(currentHealth);
        
        HotBarController.instance.UpdateToolHealthBar(HotBarController.instance.GetSelectedIndex());
    }
}
