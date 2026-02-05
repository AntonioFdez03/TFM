using System.Collections;
using UnityEngine;

public class ToolBehaviour : ItemBehaviour
{
    protected float toolDamage;
    protected float useRange = 10f;
    protected override void Awake()
    {
        base.Awake(); 
        toolDamage = 25f;
    }

    public override void Use()
    {
        if (canUse)
        {
            print("Herramienta lista para usar.");
            UseTool();
        }
    }

    protected void UseTool()
    {
        print("Buscando enemigos para golpear...");
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * useRange, Color.red);
        print("Usando herraminenta, buscando enemigos en rango...");    
        if (Physics.Raycast(ray, out hit, useRange))
        {
            print("Rayo golpeó: " + hit.collider.name);
            Enemy enemy = hit.collider.CompareTag("Enemy") ? hit.collider.GetComponent<Enemy>() : null;
            if (enemy != null)
            {
                enemy.TakeDamage(toolDamage);
                print("Enemigo golpeado con herramienta, daño: " + toolDamage);
            }
        }
        StartCoroutine(UseCooldown());
    }
}
