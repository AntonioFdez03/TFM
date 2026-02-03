using UnityEngine;

public class ResourceBehaviour : ItemBehaviour
{

    protected override void Awake()
    {
        base.Awake(); 
    }

    public override void Use()
    {
        print("Usando resource behaviour");
    }
}
