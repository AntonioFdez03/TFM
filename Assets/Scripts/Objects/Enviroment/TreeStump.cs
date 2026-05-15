using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TreeStump : HarvestableObject
{   
    
    protected override void Awake()
    {
        base.Awake();
        objectName = "TreeStump";
        maxHealth = 50;
        currentHealth = maxHealth;
        toolsAccepted.Add(ToolType.Axe);  
    }

    public override void Harvest()
    {
        Destroy(gameObject);
    }

}