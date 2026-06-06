using System;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceController : MonoBehaviour
{  
    public Action OnInventoryChanged;

    private readonly int furnaceSize = 3;
    private ItemStack[] inputItems;
    private ItemStack[] outputItems;
    private ItemStack fuelItem = null;
    private bool[] activeFuelSlots;
    private float currentFuel;
    private float timer = 0;
    private float bakeDuration = 5f;
    private int currentActiveItem = -1;
    private bool working = false;

    [SerializeField] Transform fuelPosition;
    [SerializeField] GameObject FuelLogPrefab;
    [SerializeField] GameObject FuelBranchPrefab;
    [SerializeField] GameObject FuelCharcoalPrefab;
    [SerializeField] GameObject fire;
    [SerializeField] GameObject fireLight;

    [SerializeField] private AudioClip fireSound;
    private AudioSource audioSource;

    void Start()
    {   
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = fireSound;
        audioSource.loop = true;
        audioSource.spatialBlend = 1f;
        audioSource.Play();

        inputItems = new ItemStack[furnaceSize];
        outputItems = new ItemStack[furnaceSize];
        currentFuel = 0;
        activeFuelSlots = new bool[furnaceSize];
    }

    void Update()
    {      
        SetFuelPrefab();

        if(currentFuel == 0 && fuelItem != null)
            RemoveItem(FurnaceSlotType.Fuel,0);

        if(fuelItem == null)
            currentFuel = 0;

        SetActiveItem();
        Work();

        fire.SetActive(working);
        fireLight.SetActive(working);
        
        HandleFireSound();
    }

    public ItemStack[] GetInputItems() => inputItems;
    public ItemStack[] GetOutputItems() => outputItems;
    public ItemStack GetFuelItem() => fuelItem;
    public bool[] GetActiveFuelSlots() => activeFuelSlots;
    public float GetCurrentFuel() => currentFuel;
    public float GetCurrentTimer() => timer;
    public float GetCurrentBakeDuration() => bakeDuration;
    

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

    public bool AddInput(int index, ItemStack item)
    {
        if(index < 0 || index >= furnaceSize)
            return false;

        ItemData itemData = ItemDataBase.instance.GetByID(item.id);

        FurnaceRecipe recipe = FurnaceDataBase.instance.GetRecipe(itemData);

        if(recipe == null)
            return false;

        inputItems[index] = item;

        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool AddFuel(ItemStack item)
    {
        ItemData itemData = ItemDataBase.instance.GetByID(item.id);

        if(itemData == null || fuelItem != null)
            return false;

        FuelData fuelData = FurnaceDataBase.instance.GetFuel(itemData);
        
        if(fuelData == null)
            return false;
        
        fuelItem = item;
        currentFuel = FurnaceDataBase.instance.GetFuel(itemData).energy;

        for(int i = 0; i < currentFuel; i++)
        {
            activeFuelSlots[i] = true;
        }

        OnInventoryChanged?.Invoke();
        return true;
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
        timer += Time.deltaTime;
        if(currentFuel > 0 && currentActiveItem != -1 && currentFuel >= currentActiveItem + 1)
        {   
            working = true;
            if(timer > bakeDuration)
            {
                timer = 0;
                BakeItem(currentActiveItem);
            }      
        }
        else
        {   
            working = false;
            timer = 0;
        }
    }
    private void BakeItem(int index)
    {
        if(inputItems[index] == null)
            return;
        
        ItemData item = ItemDataBase.instance.GetByID(inputItems[index].id);
        FurnaceRecipe recipe = FurnaceDataBase.instance.GetRecipe(item);

        if(recipe != null)
        {
            RemoveItem(FurnaceSlotType.Input,index);
            currentFuel -= 1;
            currentActiveItem = -1;
            activeFuelSlots[index] = false;

            float maxHealth = 0;
            if(recipe.resultItem is DurableItemData durableItemData)
                maxHealth = durableItemData.maxHealth;
        
            ItemStack resultItem = new ItemStack
            {
                id = recipe.resultItem.id,
                currentHealth = maxHealth
            };

            AddOutput(index,resultItem);
        }
    }

    private void SetActiveItem()
    {
        if(!IsInputEmpty() && currentActiveItem == -1)
        {
            for(int i = furnaceSize - 1; i >= 0; i--)
            {
                if(inputItems[i] != null && outputItems[i] == null && currentFuel >= i + 1)
                    currentActiveItem = i;
            }
        }
    }

    private void SetFuelPrefab()
    {   
        if(fuelPosition.childCount > 0)
            Destroy(fuelPosition.GetChild(0).gameObject);
        
        if(fuelItem == null) 
            return;

        switch (fuelItem.id)
        {
            case "Branch":
                InstantiateFuelPrefab(FuelBranchPrefab);
                break;

            case "Log":
                InstantiateFuelPrefab(FuelLogPrefab);
                break;

            case "Charcoal":
                InstantiateFuelPrefab(FuelCharcoalPrefab);
                break;
        }
    }

    private void InstantiateFuelPrefab(GameObject fuelPrefab)
    {
        GameObject fuelInstance = Instantiate(fuelPrefab,fuelPosition);
        fuelInstance.transform.SetLocalPositionAndRotation(Vector3.zero,Quaternion.identity);
        fuelInstance.transform.localScale = Vector3.one;
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

    private void HandleFireSound()
    {
        if(working)
        {
            if(!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if(audioSource.isPlaying)
                audioSource.Stop();
        }
    }
}