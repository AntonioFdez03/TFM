using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientUI : MonoBehaviour
{   
    [SerializeField] Image ingredientImage;
    [SerializeField] TMP_Text ingredientAmount;
    private RecipeIngredient ingredient;
    
    public void SetIngredient(RecipeIngredient newIngredient)
    {   
        ingredient = newIngredient; 

         if (ingredient == null)
            return;

        if (ingredientAmount != null && ingredient.ingredientData != null)
        {
            ingredientAmount.text = "x" + ingredient.ingredientAmount.ToString();
        }

        if (ingredientImage != null && ingredient.ingredientData != null)
        {
            ingredientImage.sprite = ingredient.ingredientData.GetItemIcon();
        }
    }
}