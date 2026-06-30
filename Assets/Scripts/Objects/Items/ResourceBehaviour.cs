using UnityEngine;
using System.Collections;

public class ResourceBehaviour : ItemBehaviour
{
    public override void Use()
    {
    }

    public override void Attack(ArmController arm)
    {
        base.Attack(arm);
        StartCoroutine(ArmController.instance.PunchMovementCR());
    }
}
