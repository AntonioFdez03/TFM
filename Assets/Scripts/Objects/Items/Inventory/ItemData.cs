using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Data")]
public class ItemData : ScriptableObject
{
    public string id;
    public string itemName;
    public Sprite icon;
    public GameObject prefab;
}

public class DurableItemData : ItemData
{
    public float maxHealth;
}

[CreateAssetMenu(menuName = "Items/Equipment Data")]
public class EquipmentData : DurableItemData
{
    public float damage;
    public float range;
}

[CreateAssetMenu(menuName = "Items/Tool Data")]
public class ToolData : EquipmentData
{
    public ToolType toolType;
}

[CreateAssetMenu(menuName = "Items/Weapon Data")]
public class WeaponData : EquipmentData
{
    
}

[CreateAssetMenu(menuName = "Items/Placeable Data")]
public class PlaceableData : DurableItemData
{
    
}

[CreateAssetMenu(menuName = "Items/Consumable Data")]
public class ConsumableData : ItemData
{
    public float hungerPoints;
    public float healthPoints;
    public float sanityPoints;
    public float consumeTime = 1f;
}

