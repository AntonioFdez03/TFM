using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class HarvestableObject : MonoBehaviour, IObjectHealth
{
    protected string objectName;
    protected int maxHealth;
    protected float currentHealth;
    protected List<ToolType> toolsAccepted = new();
    [SerializeField] protected GameObject dropItem;

    protected virtual void Awake(){}

    public string GetObjectName() => objectName;
    public float GetCurrentHealth() => currentHealth;
    public void SetCurrentHealth(float health) => currentHealth = health;
    public virtual void TakeHit(ToolType tool, float damage)
    {
        if (CanHarvest(tool))
        {
            currentHealth = Math.Clamp(currentHealth - damage, 0 ,maxHealth);
            print($"{gameObject.name} golpeado. Vida: {currentHealth}");
            
            if (currentHealth == 0)
                Harvest();
        }
        else
            print("Herramienta no válida para este objeto");   
    }

    public abstract void Harvest();
    public bool CanHarvest(ToolType tool) => toolsAccepted.Contains(tool);
    public List<ToolType> GetToolsAccepted() => toolsAccepted;
}
