using System.Collections;
using UnityEditor;
using UnityEngine;

public class WoodSpear : WeaponBehaviour
{
    protected override void Awake()
    {
        base.Awake(); 
        equipmentDamage = 50;
        equipmentRange = 5f;
        maxHealth = 100f;
        currentHealth = maxHealth;
    }

    public override void Attack(ArmController arm)
    {   
        base.Attack(arm);
        arm.StartCoroutine(arm.PunchMovementCoroutine());
    }
}
