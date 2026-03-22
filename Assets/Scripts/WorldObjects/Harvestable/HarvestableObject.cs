using System;
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

    protected virtual void Awake(){}

    public virtual void TakeHit(ToolBehaviour tool)
    {
        if (CanHarvest(tool.GetToolType()))
        {
            currentHealth = Math.Clamp(currentHealth - tool.GetToolDamage(), 0 ,maxHealth);
            print($"{gameObject.name} golpeado. Vida: {currentHealth}");
            
            if (currentHealth == 0)
                Harvest();
        }
        else
        {
            print("Herramienta no válida para este objeto");
        }
    }

    public abstract void Harvest();
    public bool CanHarvest(ToolType tool) => toolsAccepted.Contains(tool);
    public List<ToolType> GetToolsAccepted() => toolsAccepted;
}
