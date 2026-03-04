using System.Collections.Generic;
using UnityEngine;

public class CraftingController : MonoBehaviour
{   
    public static CraftingController instance;
    [SerializeField] private List<CraftingRecipe> allRecipes = new();

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public List<CraftingRecipe> GetAllRecipeList() => allRecipes;

    public List<CraftingRecipe> GetRecipeListByType(RecipeType recipeType)
    {
        List<CraftingRecipe> recipes = new();

        for(int i = 0; i < allRecipes.Count ; i++)
        {
            if(allRecipes[i].recipeType == recipeType)
                recipes.Add(allRecipes[i]);
        }
        return recipes;
    }

    public void CraftItem()
    {
        
    }

    public bool CanCraft()
    {
       return true; 
    }
}