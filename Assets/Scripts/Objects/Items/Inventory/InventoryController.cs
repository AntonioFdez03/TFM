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

    [SerializeField] private AudioClip itemPickUpSound;
    [SerializeField] private AudioClip itemDropSound;
    private AudioSource audioSource;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        items = new ItemStack[inventoryMax];

        audioSource = GetComponent<AudioSource>();
    }
    
    public float GetInventorySize() => inventoryMax;
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
        if(index >= 0 && index < inventoryMax)
        {
            items[index] = item;
            UpdateUIs();
        }
    }

    public bool CanAdd()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {   
                return true;
            }
        }
        return false;
    }

    public bool AddItem(GameObject item)
    {   
        if (item == null || !item.TryGetComponent<ItemBehaviour>(out var behaviour))
            return false;

        ItemData data = behaviour.GetData();
        ItemStack newItem = new()
        {
            id = data.id,
            currentHealth = behaviour.GetCurrentHealth()
        };

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {   
                audioSource.PlayOneShot(itemPickUpSound, 0.4f);
                items[i] = newItem;
                Destroy(item);

                UpdateUIs();
                return true;
            }
        }

        return false;
    }

    public bool AddItemFromStack(ItemStack item)
    {
        if (item == null) return false;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;

                UpdateUIs();
                return true;
            }
        }
        return false;
    }

    public void AddItemFromStack(ItemStack item, int index)
    {
        if(index < 0 || index > inventoryMax) return;

        if(items[index] == null)
            items[index] = item;
        
        UpdateUIs();
    }

    public void FastMove(int index, int origin, int max)
    {
        if(items[index] == null) return;
        ItemStack item = items[index];

        for(int i = origin; i < max; i++)
        {
            if(items[i] == null)
            {
                items[i] = item;
                items[index] = null;
                UpdateUIs();
                return;
            }
        }
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
                return;
            }
        }
    }

    public void RemoveItemByIndex(int index)
    {
        items[index] = null;
        UpdateUIs();
    }

    public bool DropItem(int index)
    {
        if(!CanDrop(index))
            return false;

        audioSource.PlayOneShot(itemDropSound, 0.4f);
        
        ItemStack itemStack = items[index];

        ItemData def = ItemDataBase.instance.GetByID(itemStack.id);
        GameObject obj = Instantiate(def.prefab, handSlot.position, Quaternion.identity);
        obj.transform.SetParent(itemsParent);

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
            rb.isKinematic = false;

            Vector3 force = CameraController.instance.transform.forward * 50f +
                            CameraController.instance.transform.up * 40f;

            rb.AddForce(force, ForceMode.Impulse);
        }

        items[index] = null;

        UpdateUIs();
        return true;
    }

    public bool CanDrop(int index)
    {
        if (index < 0 || index >= items.Length || items[index] == null)
            return false;
        

        ItemStack itemStack = items[index];

        ItemData def = ItemDataBase.instance.GetByID(itemStack.id);

        if(def.prefab.TryGetComponent<PlaceableBehaviour>(out _))
            return false;
        
        return true;
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