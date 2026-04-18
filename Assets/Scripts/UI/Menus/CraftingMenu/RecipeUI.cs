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

        if (buttonName != null && recipe.resultItem != null)
            buttonName.text = recipe.resultItem.itemName;

        if (buttonImage != null && recipe.resultItem != null)
            buttonImage.sprite = recipe.resultItem.icon;
    }

    public void OnClickCraft()
    {
        CraftingController.instance.CraftRecipe(recipe);
    }
}