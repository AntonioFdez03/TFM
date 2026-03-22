using Unity.VisualScripting;
using UnityEngine;

public enum ItemType { None, Tool, Resource, Consumable, Placeable}
[DisallowMultipleComponent]
public class ItemData : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private ItemType itemType;

    public string GetItemName() => itemName;
    public Sprite GetItemIcon() => itemIcon;
    public GameObject GetItemPrefab() => itemPrefab;
    public ItemType GetItemType() => itemType;

    public void SetItemName(string name) => itemName = name;
    public void SetItemIcon(Sprite icon) => itemIcon = icon;
    public void SetItemPrefab(GameObject prefab) => itemPrefab = prefab;
    public void SetItemType(ItemType type) => itemType = type; 

    public void CopyFrom(ItemData other)
    {
        if (other == null) return;

        itemName = other.itemName;
        itemIcon = other.itemIcon;
        itemPrefab = other.itemPrefab;
        itemType = other.itemType;
    }

    public void Clear()
    {
        itemName = null;
        itemIcon = null;
        itemPrefab = null;
        itemType = ItemType.None;
    }
}
