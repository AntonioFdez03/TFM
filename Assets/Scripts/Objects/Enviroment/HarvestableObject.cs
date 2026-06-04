using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class HarvestableObject : MonoBehaviour, IObjectHealth
{   
    [SerializeField] protected GameObject dropItem = null;
    protected string objectName;
    protected int maxHealth;
    protected float currentHealth;
    protected List<ToolType> toolsAccepted = new();

    [SerializeField] private AudioClip hitSound;
    [SerializeField] protected AudioClip harvestSound;

    public string GetObjectName() => objectName;
    public float GetCurrentHealth() => currentHealth;
    public void SetCurrentHealth(float health) => currentHealth = health;
    public virtual void TakeHit(ToolType tool, float damage)
    {
        Debug.Log("takeHit: " + tool + ", damage: " + damage);
        if (CanHarvest(tool))
        {
            currentHealth = Math.Clamp(currentHealth - damage, 0 ,maxHealth);
            
            if (currentHealth == 0)
            {
                AudioManager.instance.PlayOneShot(harvestSound, 0.6f);
                Harvest();
            }
            else if(hitSound != null)
                AudioManager.instance.PlayOneShot(hitSound, 0.4f);
        }
        else
            print("Herramienta no válida para este objeto");   
    }

    public abstract void Harvest();
    public bool CanHarvest(ToolType tool) => toolsAccepted.Contains(tool);
    public List<ToolType> GetToolsAccepted() => toolsAccepted;
}
