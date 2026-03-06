using UnityEngine;

[DisallowMultipleComponent]
public class ItemData : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private GameObject itemPrefab;

    public string GetItemName() => itemName;
    public Sprite GetItemIcon() => itemIcon;
    public GameObject GetItemPrefab() => itemPrefab;

    public void SetItemName(string name) => itemName = name;
    public void SetItemIcon(Sprite icon) => itemIcon = icon;
    public void SetItemPrefab(GameObject prefab) => itemPrefab = prefab;

    public void CopyFrom(ItemData other)
    {
        if (other == null) return;

        itemName = other.itemName;
        itemIcon = other.itemIcon;
        itemPrefab = other.itemPrefab;
    }

    public void Clear()
    {
        itemName = null;
        itemIcon = null;
        itemPrefab = null;
    }
}
