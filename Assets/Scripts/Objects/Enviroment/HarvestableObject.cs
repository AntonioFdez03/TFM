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
    protected AudioSource audioSource;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public string GetObjectName() => objectName;
    public float GetCurrentHealth() => currentHealth;
    public void SetCurrentHealth(float health) => currentHealth = health;
    public virtual void TakeHit(ToolType tool, float damage)
    {
        Debug.Log("takeHit: " + tool + ", damage: " + damage);
        if (CanHarvest(tool))
        {
            Debug.Log("Entra");
            currentHealth = Math.Clamp(currentHealth - damage, 0 ,maxHealth);

            if(hitSound != null && audioSource != null)
                audioSource.PlayOneShot(hitSound);
            
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
