using System.Collections.Generic;
using UnityEngine;

public class FurnaceDataBase : MonoBehaviour
{
    public static FurnaceDataBase instance;
    public List<FurnaceRecipe> recipes;
    public List<FuelData> fuels;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    
    public FurnaceRecipe GetRecipe(ItemData input)
    {
        return recipes.Find(r => r.inputItem == input);
    }

    public FuelData GetFuel(ItemData fuel)
    {
        return fuels.Find(f => f.fuelItem == fuel);
    }
}