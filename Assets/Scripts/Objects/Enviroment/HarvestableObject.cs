using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class HarvestableObject : MonoBehaviour, IObjectHealth
{   
    [SerializeField] protected HarvestableData data;
    [SerializeField] protected GameObject dropItem = null;
    protected string objectName;
    protected float currentHealth;

    [SerializeField] private AudioClip hitSound;
    [SerializeField] protected AudioClip harvestSound;
    protected bool initialized = false;

    public void Initialize(float health)
    {
        if (health > 0)
            currentHealth = health;
        else
            currentHealth = data.maxHealth;

        initialized = true;
    }

    public HarvestableData GetData() => data;
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => data.maxHealth;
    public void SetCurrentHealth(float health) => currentHealth = health;
    public virtual void TakeHit(ToolType tool, float damage)
    {
        if (CanHarvest(tool))
        {   
            if(hitSound != null)
                AudioManager.instance.PlayOneShot(hitSound, 0.9f);
                
            currentHealth = Math.Clamp(currentHealth - damage, 0 ,data.maxHealth);
            
            if(TryGetComponent(out ItemBehaviour itemB))
                itemB.SetCurrentHealth(currentHealth);

            if (currentHealth == 0)
            {
                AudioManager.instance.PlayOneShot(harvestSound, 0.6f);
                Harvest();
            }
        }
        else
            print("Herramienta no válida para este objeto");   
    }

    public abstract void Harvest();
    public bool CanHarvest(ToolType tool) => data.toolsAccepted.Contains(tool);
}
