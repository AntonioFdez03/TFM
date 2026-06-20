using System;
using System.Collections.Generic;
using UnityEngine;

public class StorageController : MonoBehaviour
{  
    public Action OnInventoryChanged;

    private int storageSize = 15;
    private ItemStack[] items;

    void Start()
    {
        items = new ItemStack[storageSize];
       
    }

    void Update()
    {
        //print("Working...");
    }

    public int GetStorageSize() => storageSize;
    public ItemStack GetItem(int index)
    {
        if(index < 0 || index > storageSize) return null;
        return items[index];
    }

    public ItemStack[] GetItems() => items;

    public void AddItem(int index, ItemStack item)
    {   
        if(index >= 0 && index < storageSize)
        {
            items[index] = item;
        }

        OnInventoryChanged?.Invoke();
    }

    public bool AddItem(ItemStack item)
    {
        for(int i = 0; i < storageSize; i++)
        {
            if(items[i] == null)
            {
                items[i] = item;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }
        return false;
    }

    public void RemoveItem(int index)
    {   
        if(index < 0 || index > storageSize) return;
        items[index] = null;
        OnInventoryChanged?.Invoke();
    }

    public void SwapItems(int originIndex, int targetIndex)
    {
        if (originIndex == targetIndex) return;

        (items[targetIndex], items[originIndex]) = (items[originIndex], items[targetIndex]);
        OnInventoryChanged?.Invoke();
    }

    public bool isEmpty()
    {
        for(int i = 0; i < storageSize ; i++)
        {
            if(items[i] != null)
                return false;
        }
        return true;
    }
}