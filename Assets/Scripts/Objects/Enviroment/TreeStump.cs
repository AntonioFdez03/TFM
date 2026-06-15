using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TreeStump : HarvestableObject
{   
    
    protected void Awake()
    {
        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
    }

    public override void Harvest()
    {
        Destroy(gameObject);
    }

}