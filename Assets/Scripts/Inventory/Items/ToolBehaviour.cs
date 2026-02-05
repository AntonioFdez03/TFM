using System.Collections;
using UnityEngine;

public class ToolBehaviour : ItemBehaviour
{
    protected float toolDamage;
    protected float useRange = 5f;
    protected override void Awake()
    {
        base.Awake(); 
        toolDamage = 25f;
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

        Debug.DrawRay(ray.origin, ray.direction * useRange, Color.red);
        if (Physics.Raycast(ray, out hit, useRange))
        {
            Enemy enemy = hit.collider.CompareTag("Enemy") ? hit.collider.GetComponent<Enemy>() : null;
            if (enemy != null)
                enemy.TakeDamage(toolDamage);
        }
    }
}
