using System.Collections;
using UnityEditor;
using UnityEngine;

public class StoneAxe : ToolBehaviour
{
    protected override void Awake()
    {
        base.Awake(); 
        toolType = ToolType.Axe;
        toolDamage = 20;
        toolRange = 5f;
        maxhealth = 100f;
        currentHealth = maxhealth;
    }
}
