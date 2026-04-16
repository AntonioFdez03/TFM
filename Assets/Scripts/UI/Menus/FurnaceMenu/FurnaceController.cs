using System;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceController : MonoBehaviour
{  
    public Action OnInventoryChanged;

    private int furnaceSize = 3;
    private ItemData[] input;
    private ItemData[] output;
    private ItemData fuelObject = null;
    private float currentFuel;
    private float maxFuel = 100;
    private float fuelBurnRate = 2f;

    void Start()
    {
        input = new ItemData[furnaceSize];
        output = new ItemData[furnaceSize];
        currentFuel = 0;
    }

    void Update()
    {
        //print("Working...");
    }

    public ItemData[] GetInputObjects() => input;
    public ItemData[] GetOutputObjects() => output;
    public float GetCurrentFuel() => currentFuel;

    public void AddInput(int index, ItemData item)
    {   
        if(index >= 0 && index < furnaceSize)
        {
            input[index] = item;
            print("Objeto añadido: " + item.GetItemName() + " en el index: " + index);
        }

        print("Entra");
        OnInventoryChanged?.Invoke();
    }

    public void AddFuel()
    {
        String fuelName = fuelObject.GetComponent<ItemData>().GetItemName();
    }
}