using UnityEngine;

public enum ItemType { None, Resource, Weapon, Consumable, Tool }

[DisallowMultipleComponent]
public class ItemData : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private ItemType itemType;
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private GameObject itemPrefab;

    public string GetItemName() => itemName;
    public ItemType GetItemType() => itemType;
    public Sprite GetItemIcon() => itemIcon;
    public GameObject GetItemPrefab() => itemPrefab;

    public void SetItemName(string name) => itemName = name;
    public void SetItemType(ItemType type) => itemType = type;
    public void SetItemIcon(Sprite icon) => itemIcon = icon;
    public void SetItemPrefab(GameObject prefab) => itemPrefab = prefab;

    public void CopyFrom(ItemData other)
    {
        if (other == null) return;

        itemName = other.itemName;
        itemType = other.itemType;
        itemIcon = other.itemIcon;
        itemPrefab = other.itemPrefab;
    }

    public void Clear()
    {
        itemName = null;
        itemType = default;
        itemIcon = null;
        itemPrefab = null;
    }
}
