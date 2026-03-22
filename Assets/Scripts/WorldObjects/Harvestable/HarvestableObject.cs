using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class HarvestableObject : MonoBehaviour
{
    protected int maxHealth;
    protected float currentHealth;
    protected List<ToolType> toolsAccepted = new();
    [SerializeField] protected GameObject dropItem;
    protected int dropItemsCount;
    protected float cooldownToDrop;

    protected abstract void Awake();

    public void TakeHit(ToolBehaviour tool)
    {
        if (CanHarvest(tool.GetToolType()))
        {
            currentHealth -= tool.GetToolDamage();
            print($"{gameObject.name} golpeado. Vida: {currentHealth}");
            
            if (currentHealth <= 0)
                Harvest();
        }
        else
        {
            print("Herramienta no válida para este objeto");
        }
    }

    public abstract void Harvest();
    protected abstract void GenerateDropItems();
    public bool CanHarvest(ToolType tool) => toolsAccepted.Contains(tool);
    public List<ToolType> GetToolsAccepted() => toolsAccepted;
}
