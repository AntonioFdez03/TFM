using Unity.VisualScripting;
using UnityEngine;

public enum ItemType { Resource, Weapon, Consumable, Tool }

public class ItemData : MonoBehaviour
{
    [SerializeField] string itemName;
    [SerializeField] ItemType itemType;
    [SerializeField] Sprite itemIcon;
    [SerializeField] GameObject itemPrefab;

    public string GetItemName() => itemName;
    public ItemType GetItemType() => itemType;
    public Sprite GetItemIcon() => itemIcon;
    public GameObject GetItemPrefab() => itemPrefab;

    public void SetItemName(string name) => itemName = name;
    public void SetItemType(ItemType type) => itemType = type;
    public void SetItemIcon(Sprite icon) => itemIcon = icon;
    public void SetItemPrefab(GameObject prefab) => itemName = name;
}