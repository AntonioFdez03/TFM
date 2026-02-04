using System.Collections;
using UnityEngine;

public class ToolBehaviour : ItemBehaviour
{
    protected float toolDamage;
    protected float useRange = 5f;
    protected float useCooldown = 0.5f;
    protected override void Awake()
    {
        base.Awake(); 
        toolDamage = 25f;
    }

    public override void Use()
    {
        print("Usando herramienta...");
        StartCoroutine(UseCooldown());
    }

    IEnumerator UseCooldown()
    {
        yield return new WaitForSeconds(useCooldown);
        print("Herramienta usada");
        UseTool();
    }

    private void UseTool()
    {
        print("Buscando enemigos para golpear...");
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * useRange, Color.red);
        print("Usando herraminenta, buscando enemigos en rango...");    
        if (Physics.Raycast(ray, out hit, useRange))
        {
            Enemy enemy = hit.collider.CompareTag("Enemy") ? hit.collider.GetComponent<Enemy>() : null;
            if (enemy != null)
            {
                enemy.TakeDamage(toolDamage);
                print("Enemigo golpeado con herramienta, daño: " + toolDamage);
            }
        }
    }
}
