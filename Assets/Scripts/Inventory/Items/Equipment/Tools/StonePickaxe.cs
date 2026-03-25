using System.Collections;
using UnityEditor;
using UnityEngine;

public class StonePickaxe : ToolBehaviour
{
    protected override void Awake()
    {
        base.Awake(); 
        toolType = ToolType.Pickaxe;
        equipmentDamage = 15;
        equipmentRange = 5f;
        maxHealth = 100f;
        currentHealth = maxHealth;
    }
}
