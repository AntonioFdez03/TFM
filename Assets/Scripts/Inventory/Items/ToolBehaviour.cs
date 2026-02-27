using System.Collections;
using UnityEditor;
using UnityEngine;

public enum ToolType { Axe, Pickaxe}
public class ToolBehaviour : ItemBehaviour
{
    protected ToolType toolType;
    protected int toolDamage;
    protected float toolRange = 5f;

    public ToolType GetToolType() => toolType;
    public int GetToolDamage() => toolDamage;

    protected override void Awake()
    {
        base.Awake(); 
    }

    public override void Use()
    {
        Debug.Log($"ToolBehaviour instance ID: {GetInstanceID()}");
        if (!canUse) 
        {
            print("Herramienta en cooldown.");
            return;
        }

        canUse = false;          // 🔒 BLOQUEAS AQUÍ
        print("Herramienta usada.");
        UseTool();
        StartCoroutine(UseCooldown());
    }

    protected void UseTool()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * toolRange, Color.red);
        if (Physics.Raycast(ray, out hit, toolRange))
        {
            Enemy enemy = hit.collider.CompareTag("Enemy") ? hit.collider.GetComponent<Enemy>() : null;
            if (enemy != null)
                enemy.TakeDamage(toolDamage);

            HarvestableObject harvestableObject = hit.collider.CompareTag("Harvestable") ? hit.collider.GetComponent<HarvestableObject>() : null;
            if(harvestableObject != null)
            {
                harvestableObject.TakeHit(this);
            }
        }
    }
}
