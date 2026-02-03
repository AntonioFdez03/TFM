using UnityEngine;
public enum ItemType {Resource, Tool, Consumable}
public class ItemData : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    public GameObject itemPrefab;
}
