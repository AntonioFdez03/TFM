using UnityEngine;

public class ToolBehaviour : ItemBehaviour
{
    [Header("Tool settings")]
    [SerializeField] float useCooldown = 0.5f;
    private float lastUseTime;

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
