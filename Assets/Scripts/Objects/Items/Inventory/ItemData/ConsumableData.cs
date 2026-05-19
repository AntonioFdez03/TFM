using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumable Data")]
public class ConsumableData : ItemData
{
    public float hungerPoints;
    public float healthPoints;
    public float sanityPoints;
    public float consumeTime = 1f;
}