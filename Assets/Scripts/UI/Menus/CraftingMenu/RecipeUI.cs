using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour
{   
    [SerializeField] Image buttonImage;
    [SerializeField] TMP_Text buttonName;
    private CraftingRecipe recipe;
    public void SetRecipe(CraftingRecipe newRecipe)
    {
        recipe = newRecipe;

         if (recipe == null)
            return;

        if (buttonName != null && recipe.recipeItem != null)
            buttonName.text = recipe.recipeItem.GetComponent<ItemData>().GetItemName();

        if (buttonImage != null && recipe.recipeItem != null)
            buttonImage.sprite = recipe.recipeItem.GetComponent<ItemData>().GetItemIcon();
    }

    public void OnClickCraft()
    {
        CraftingController.instance.CraftRecipe(recipe);
    }
}