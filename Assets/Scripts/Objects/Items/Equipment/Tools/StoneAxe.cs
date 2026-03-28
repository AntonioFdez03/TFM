using System.Collections;
using UnityEditor;
using UnityEngine;

public class StoneAxe : ToolBehaviour
{
    protected override void Awake()
    {
        base.Awake(); 
        toolType = ToolType.Axe;
        equipmentDamage = 20;
        equipmentRange = 5f;
        maxHealth = 100f;
        currentHealth = maxHealth;
    }
}
