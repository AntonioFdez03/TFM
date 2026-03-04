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

    public void CraftRecipe(CraftingRecipe recipe)
    {
        if (CanCraft(recipe))
        {
            foreach(RecipeIngredient ingredient in recipe.ingredients)
                InventoryController.instance.RemoveItem(InventoryController.instance.FindItemByName(ingredient.ingredientData.GetItemName()));
            
            InventoryController.instance.AddItem(recipe.recipeItem);
        }else
            print("NO SE PUEDE CRAFTEAR");
    }

    public bool CanCraft(CraftingRecipe recipe)
    {
       foreach(RecipeIngredient ingredient in recipe.ingredients)
        {
            if(InventoryController.instance.FindItemByName(ingredient.ingredientData.GetItemName()) == null)
                return false;
        }
        return true;
    }
}