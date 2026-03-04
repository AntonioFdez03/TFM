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

        if (buttonName != null && recipe.recipeData != null)
            buttonName.text = recipe.recipeData.GetItemName();

        if (buttonImage != null && recipe.recipeData != null)
            buttonImage.sprite = recipe.recipeData.GetItemIcon();
    }
}