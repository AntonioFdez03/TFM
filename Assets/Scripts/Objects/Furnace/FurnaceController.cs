using System;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceController : MonoBehaviour
{  
    public Action OnFurnaceChanged;

    private readonly int furnaceSize = 3;
    private ItemStack[] inputItems;
    private ItemStack[] outputItems;
    private ItemStack fuelItem = null;
    private bool[] activeFuelSlots;
    private int currentFuel;
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
        {
            currentFuel = 0;
            SetActiveFuelSlots();
        }

        
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

        OnFurnaceChanged?.Invoke();
        return true;
    }

    public bool AddInputFast(ItemStack item)
    {
        if(item == null)
            return false;

        ItemData itemData = ItemDataBase.instance.GetByID(item.id);
        FurnaceRecipe recipe = FurnaceDataBase.instance.GetRecipe(itemData);

        if(recipe == null)
            return false;

        for(int i = 0; i < furnaceSize; i++)
        {
            if(inputItems[i] == null)
            {
                inputItems[i] = item;
                OnFurnaceChanged?.Invoke();
                return true;
            }
        }
        return false;
    }

    public bool AddFuel(ItemStack item)
    {
        if(item == null) return false;

        ItemData itemData = ItemDataBase.instance.GetByID(item.id);

        if(itemData == null || fuelItem != null)
            return false;

        FuelData fuelData = FurnaceDataBase.instance.GetFuel(itemData);
        
        if(fuelData == null)
            return false;
        
        fuelItem = item;
        currentFuel = (int)(fuelItem.currentHealth / 10);

        SetActiveFuelSlots();

        OnFurnaceChanged?.Invoke();
        return true;
    }

    private void AddOutput(int index, ItemStack item)
    {
        if(index >= 0 && index < furnaceSize)
        {
            outputItems[index] = item;
        }

        OnFurnaceChanged?.Invoke();
    }

    public void RemoveItem(FurnaceSlotType type, int index)
    {   
        if(index < 0 || index > furnaceSize) return;

        switch (type)
        {
            case FurnaceSlotType.Input:
                inputItems[index] = null;

                if(index == currentActiveItem)
                    currentActiveItem = -1;
                break;

            case FurnaceSlotType.Fuel:
                fuelItem = null;
                break;

            case FurnaceSlotType.Output:
                outputItems[index] = null;
                break;
        }
        OnFurnaceChanged?.Invoke();
    }

    private void Work()
    {
        if(currentActiveItem == -1) 
        {
            timer = 0;
            working = false;
            return;
        }
        timer += Time.deltaTime;

        if(activeFuelSlots[currentActiveItem])
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
            fuelItem.currentHealth = Math.Clamp(fuelItem.currentHealth - 10, 0, furnaceSize * 10);
            activeFuelSlots[index] = false;

            if(fuelItem.currentHealth == 0)
                fuelItem = null;

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
            for(int i = 0; i < furnaceSize; i++)
            {   
                if(inputItems[i] != null && outputItems[i] == null)
                {   
                    currentActiveItem = i;
                    return;
                }
            }
        }
    }

    private void SetActiveFuelSlots()
    {
        for(int i = 0; i < furnaceSize; i++)
        {
            activeFuelSlots[i] = i < currentFuel;
        }

        OnFurnaceChanged?.Invoke();
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