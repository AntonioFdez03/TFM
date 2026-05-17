using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{   
    public static InventoryController instance;
    public static Action OnInventoryChanged;

    [SerializeField] Transform itemsParent; 
    [SerializeField] Transform handSlot;

    private int inventoryMax = 28;
    private int hotBarSize = 7;
    private int inventoryGridSize = 21;

    private ItemStack[] items;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        items = new ItemStack[inventoryMax];
    }
    
    public void SetInventoryItems(ItemStack[] newItems) => items = newItems;
    public ItemStack[] GetInventoryItems() => items;
    public int GetHotBarSize() => hotBarSize;
    public int GetInventoryGridSize() => inventoryGridSize;
    public Transform GetItemsParent() => itemsParent;

    public ItemStack GetItem(int index)
    {
        if(index < 0 || index > inventoryMax) return null;
        return items[index];
    }

    public void SetItem(int index, ItemStack item)
    {
        if(index >= 0 && index < inventoryMax && items[index] == null && item != null)
        {
            items[index] = item;
            UpdateUIs();
        }
    }

    public bool AddItem(GameObject item)
    {   
        if (item == null || !item.TryGetComponent<ItemBehaviour>(out var behaviour))
            return false;

        ItemData data = behaviour.GetData();
        print("Vida del item: " + behaviour.GetCurrentHealth());
        ItemStack newItem = new()
        {
            id = data.id,
            currentHealth = behaviour.GetCurrentHealth()
        };

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = newItem;
                Destroy(item);

                UpdateUIs();
                return true;
            }
        }

        return false;
    }

    public void AddItemFromStack(ItemStack item)
    {
        if (item == null) return;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;

                UpdateUIs();
                return;
            }
        }
    }

    public void AddItemFromStack(ItemStack item, int index)
    {
        if(index < 0 || index > inventoryMax) return;

        if(items[index] == null)
            items[index] = item;
        
        UpdateUIs();
    }

    public void RemoveItem(ItemStack item)
    {
        if (instance == null) return;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == item)
            {
                items[i] = null;
                UpdateUIs();
                return;
            }
        }
    }

    public void RemoveItemById(string id)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].id == id)
            {
                items[i] = null;

                UpdateUIs();
            }
        }
    }

    public void DropItem(int index)
    {
        if (index < 0 || index >= items.Length || items[index] == null)
            return;

        ItemStack itemStack = items[index];

        ItemData def = ItemDataBase.instance.GetByID(itemStack.id);

        GameObject obj = Instantiate(def.prefab, handSlot.position, Quaternion.identity);

        // posición / rotación
        Transform player = PlayerController.instance.transform;
        obj.transform.rotation = player.rotation * Quaternion.Euler(0f, 0f, 90f);

        // restaurar estado
        ItemBehaviour behaviour = obj.GetComponent<ItemBehaviour>();
        if (behaviour != null)
            behaviour.Initialize(itemStack);

        // física
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.isKinematic = false;

            Vector3 force = CameraController.instance.transform.forward * 50f +
                            CameraController.instance.transform.up * 40f;

            rb.AddForce(force, ForceMode.Impulse);
        }

        items[index] = null;

        UpdateUIs();
    }

    public void SwapItems(int originIndex, int targetIndex)
    {
        if (originIndex == targetIndex) return;

        (items[targetIndex], items[originIndex]) = (items[originIndex], items[targetIndex]);
        UpdateUIs();
    }

    public List<ItemStack> GetItemsById(string id)
    {
        List<ItemStack> result = new();

        foreach (var item in items)
        {
            if (item == null) continue;

            ItemData def = ItemDataBase.instance.GetByID(item.id);

            if (def != null && def.id == id)
                result.Add(item);
        }

        return result;
    }

    private void UpdateUIs()
    {
        OnInventoryChanged?.Invoke();
        HotBarController.instance.UpdateHotBarUI();
    }
}