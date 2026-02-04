using UnityEngine;

public class ToolBehaviour : ItemBehaviour
{
    protected override void Awake()
    {
        base.Awake(); 
    }

    public override void Use()
    {
        print("Usando tool behaviour");
    }

    private void UseTool()
    {
        // lógica concreta del tool
        // ej: detectar tronco, reducir vida, etc.
    }
}
