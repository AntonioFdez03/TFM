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
    }

    public List<CraftingRecipe> GetAllRecipeList() => allRecipes;
    public void SetStationType(CraftingStationType craftingStationType) => stationType = craftingStationType;
    public CraftingStationType GetStationType() => stationType;

    public void CraftRecipe(CraftingRecipe recipe)
    {
        if (!CanCraft(recipe))
        {
            Debug.Log("NO SE PUEDE CRAFTEAR");
            return;
        }

        // 1. consumir ingredientes
        foreach (RecipeIngredient ingredient in recipe.ingredients)
        {
            int remaining = ingredient.ingredientAmount;

            for (int i = 0; i < InventoryController.instance.GetInventoryItems().Length; i++)
            {
                var item = InventoryController.instance.GetInventoryItems()[i];
                if (item == null) continue;

                if (item.id == ingredient.ingredientData.id)
                {
                    InventoryController.instance.RemoveItemById(item.id);
                    remaining--;

                    if (remaining <= 0)
                        break;
                }
            }
        }

        // 2. crear ItemInstance nuevo
        ItemStack newItem = new ItemStack
        {
            id = recipe.resultItem.id,
            currentHealth = recipe.resultItem.maxHealth
        };

        // 3. añadir al inventario
        InventoryController.instance.AddItemFromStack(newItem);
    }

    public bool CanCraft(CraftingRecipe recipe)
    {
       foreach(RecipeIngredient ingredient in recipe.ingredients)
        {
            if(InventoryController.instance.GetItemsById(ingredient.ingredientData.id).Count < ingredient.ingredientAmount)
                return false;
        }
        return true;
    }
}