using System.Collections.Generic;
using UnityEngine;

public class CraftingController : MonoBehaviour
{   
    public static CraftingController instance;
    [SerializeField] private List<CraftingRecipe> allRecipes = new();
    private CraftingStationType stationType;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        print(allRecipes[2]);
    }

    public List<CraftingRecipe> GetAllRecipeList() => allRecipes;
    public void SetStationType(CraftingStationType craftingStationType) => stationType = craftingStationType;
    public CraftingStationType GetStationType() => stationType;

    public void CraftRecipe(CraftingRecipe recipe)
    {
        if (CanCraft(recipe))
        {
            foreach(RecipeIngredient ingredient in recipe.ingredients)
                for(int i = 0 ; i < ingredient.ingredientAmount ; i++)
                    InventoryController.instance.RemoveItem(InventoryController.instance.FindItemByName(ingredient.ingredientData.GetItemName()));
            
            GameObject itemInstance = Instantiate(recipe.recipeItem);
            InventoryController.instance.AddItem(itemInstance);
        }else
            print("NO SE PUEDE CRAFTEAR");
    }

    public bool CanCraft(CraftingRecipe recipe)
    {
       foreach(RecipeIngredient ingredient in recipe.ingredients)
        {
            if(InventoryController.instance.GetItemAmount(ingredient.ingredientData.GetItemName()) < ingredient.ingredientAmount)
                return false;
        }
        return true;
    }
}