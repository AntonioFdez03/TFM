using UnityEngine;

public class Spear : WeaponBehaviour
{
    public override void Attack(ArmController arm)
    {
        base.Attack(arm);

        if (!canUse)
            return;

        //canUse = false;
        StartCoroutine(UseCooldown());

        arm.StartCoroutine(arm.SpearMovementCR());
    }
}