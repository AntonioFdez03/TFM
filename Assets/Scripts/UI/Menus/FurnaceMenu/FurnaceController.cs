using System;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceController : MonoBehaviour
{  
    public Action OnInventoryChanged;

    private int furnaceSize = 3;
    private ItemStack[] input;
    private ItemStack[] output;
    private ItemStack fuelObject = null;
    private float currentFuel;
    private float maxFuel = 100;
    private float fuelBurnRate = 2f;

    void Start()
    {
        input = new ItemStack[furnaceSize];
        output = new ItemStack[furnaceSize];
        currentFuel = 0;
    }

    void Update()
    {
        //print("Working...");
    }

    public ItemStack[] GetInputObjects() => input;
    public ItemStack[] GetOutputObjects() => output;
    public ItemStack GetFuelObject() => fuelObject;
    public float GetCurrentFuel() => currentFuel;

    public ItemStack GetItem(FurnaceSlotType type,int index)
    {
        if(index < 0 || index > furnaceSize) return null;

        ItemStack item = null;
        switch (type)
        {
            case FurnaceSlotType.Input:
                item = input[index];
                break;

            case FurnaceSlotType.Fuel:
                item = fuelObject;
                break;

            case FurnaceSlotType.Output:
                item = output[index];
                break;
        }
        return item;
    }

    public void AddInput(int index, ItemStack item)
    {   
        if(index >= 0 && index < furnaceSize)
        {
            input[index] = item;
        }

        OnInventoryChanged?.Invoke();
    }

    public void AddFuel(ItemStack item)
    {
        fuelObject = item;
        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(FurnaceSlotType type, int index)
    {   
        if(index < 0 || index > furnaceSize) return;

        switch (type)
        {
            case FurnaceSlotType.Input:
                input[index] = null;
                break;

            case FurnaceSlotType.Fuel:
                fuelObject = null;
                break;

            case FurnaceSlotType.Output:
                output[index] = null;
                break;
        }
        OnInventoryChanged?.Invoke();
    }
}