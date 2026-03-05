using System.Collections;
using UnityEditor;
using UnityEngine;

public class Axe : ToolBehaviour
{
    protected override void Awake()
    {
        base.Awake(); 
        toolType = ToolType.Axe;
        toolDamage = 20;
    }
}
