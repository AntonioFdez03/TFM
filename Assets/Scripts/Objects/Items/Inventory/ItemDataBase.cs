using System.Collections.Generic;
using UnityEngine;

public enum ItemType { None, Tool, Weapon, Resource, Consumable, Placeable}
public class ItemDataBase : MonoBehaviour
{
    public static ItemDataBase instance;

    [SerializeField] private List<ItemData> items;

    private Dictionary<string, ItemData> map;

    void Awake()
    {
        instance = this;

        map = new Dictionary<string, ItemData>();

        foreach (var item in items)
        {
            map[item.id] = item;
        }
    }

    public ItemData GetByID(string id)
    {
        map.TryGetValue(id, out ItemData data);
        return data;
    }
}