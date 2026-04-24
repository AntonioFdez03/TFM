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

    void Start()
    {
        inputItems = new ItemStack[furnaceSize];
        outputItems = new ItemStack[furnaceSize];
        currentFuel = 0;
    }

    void Update()
    {
        if(currentFuel > 0 && !IsInputEmpty())
        {   
            print("Working");
            Work();
        }
    }

    public ItemStack[] GetInputItems() => inputItems;
    public ItemStack[] GetOutputItems() => outputItems;
    public ItemStack GetFuelItem() => fuelItem;
    public float GetCurrentFuel() => currentFuel;

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
        ItemData itemData = ItemDataBase.instance.GetByID(fuelItem.id);
        currentFuel = FurnaceDataBase.instance.GetFuel(itemData).energy;
        OnInventoryChanged?.Invoke();
    }

    private void AddOutput(int index, ItemStack item)
    {
        if(index >= 0 && index < furnaceSize)
        {
            outputItems[index] = item;
        }

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

    private void Work()
    {   
        if(inputItems[0] == null)
            return;
        
        ItemData item = ItemDataBase.instance.GetByID(inputItems[0].id);
        FurnaceRecipe recipe = FurnaceDataBase.instance.GetRecipe(item);

        if(recipe != null)
        {
            RemoveItem(FurnaceSlotType.Input,0);
            RemoveItem(FurnaceSlotType.Fuel,0);
            
            // 2. crear ItemInstance nuevo
            ItemStack newItem = new ItemStack
            {
                id = recipe.resultItem.id,
                currentHealth = recipe.resultItem.maxHealth
            };

          
            AddOutput(0,newItem);
        }
    }

    public bool IsFurnaceEmpty()
    {   
        if(fuelItem != null) return false;

        for(int i = 0; i < furnaceSize ; i++)
        {
            if(inputItems[i] != null || outputItems[i] != null)
                return false;
        }

        return true;

    }

    private bool IsInputEmpty()
    {
        for(int i = 0; i < furnaceSize ; i++)
        {
            if(inputItems[i] != null)
                return false;
        }

        return true;
    }
}