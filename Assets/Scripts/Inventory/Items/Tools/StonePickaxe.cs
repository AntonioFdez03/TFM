using System.Collections;
using UnityEditor;
using UnityEngine;

public class StonePickaxe : ToolBehaviour
{
    protected override void Awake()
    {
        base.Awake(); 
        toolType = ToolType.Pickaxe;
        toolDamage = 15;
        toolRange = 5f;
        maxhealth = 100f;
        currentHealth = maxhealth;
    }
}
