using UnityEngine;
using System.Collections;

public abstract class ConsumableBehaviour : ItemBehaviour
{   
    protected int itemUses;
    protected override void Awake()
    {
        base.Awake(); 
    }

    public override void Use()
    {
        print("Usando resource behaviour");
    }

    protected abstract void Consume();
}
