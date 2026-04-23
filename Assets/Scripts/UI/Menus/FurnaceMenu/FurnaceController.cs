using System;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceController : MonoBehaviour
{  
    public Action OnInventoryChanged;

    private int furnaceSize = 3;
    private ItemStack[] inputItems;
    private ItemStack[] outputItems;
    private ItemStack fuelItem = null;
    private float currentFuel;
    private float maxFuel = 100;
    private float fuelBurnRate = 2f;

    void Start()
    {
        inputItems = new ItemStack[furnaceSize];
        outputItems = new ItemStack[furnaceSize];
        currentFuel = 0;
    }

    void Update()
    {
        //print("Working...");
    }

    public ItemStack[] GetInputItems() => inputItems;
    public ItemStack[] GetOutputItems() => outputItems;
    public ItemStack GetFuelItem() => fuelItem;
    public float GetCurrentFuel() => currentFuel;

    public bool IsEmpty()
    {   
        if(fuelItem != null) return false;

        for(int i = 0; i < furnaceSize ; i++)
        {
            if(GetItem(FurnaceSlotType.Input,i) != null || GetItem(FurnaceSlotType.Output,i) != null)
                return false;
        }

        return true;

    }

    public ItemStack GetItem(FurnaceSlotType type,int index)
    {
        if(index < 0 || index > furnaceSize) return null;

        ItemStack item = null;
        switch (type)
        {
            case FurnaceSlotType.Input:
                item = inputItems[index];
                break;

            case FurnaceSlotType.Fuel:
                item = fuelItem;
                break;

            case FurnaceSlotType.Output:
                item = outputItems[index];
                break;
        }
        return item;
    }

    public void AddInput(int index, ItemStack item)
    {   
        if(index >= 0 && index < furnaceSize)
        {
            inputItems[index] = item;
        }

        OnInventoryChanged?.Invoke();
    }

    public void AddFuel(ItemStack item)
    {
        fuelItem = item;
        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(FurnaceSlotType type, int index)
    {   
        if(index < 0 || index > furnaceSize) return;

        switch (type)
        {
            case FurnaceSlotType.Input:
                inputItems[index] = null;
                break;

            case FurnaceSlotType.Fuel:
                fuelItem = null;
                break;

            case FurnaceSlotType.Output:
                outputItems[index] = null;
                break;
        }
        OnInventoryChanged?.Invoke();
    }
}