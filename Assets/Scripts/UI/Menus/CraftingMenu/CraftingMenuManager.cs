using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Unity.VisualScripting;

public class CraftingMenuManager : MonoBehaviour
{   
    [SerializeField] private GameObject recipesPanel;
    [SerializeField] private GameObject recipePrefab;
    [SerializeField] private GameObject ingredientPrefab;

    void Start()
    {   
        CleanRecipes();
        ShowRecipes(RecipeType.None);
    }

    void OnEnable()
    {
        CleanRecipes();
        ShowRecipes(RecipeType.None);
    }

    private void CleanRecipes()
    {
        foreach (Transform child in recipesPanel.transform)
            Destroy(child.gameObject);
        
    }

    public void ShowRecipes(RecipeType recipeType)
    {
        CleanRecipes();
        foreach(CraftingRecipe recipe in CraftingController.instance.GetAllRecipeList())
        {
            if((recipeType == RecipeType.None || recipe.recipeType == recipeType) && (recipe.stationType == CraftingController.instance.GetStationType()))
                ShowRecipe(recipe);
        }
    }

    public void ShowRecipe(CraftingRecipe recipe)
    {
        if(recipesPanel == null ||recipePrefab == null )
            return;

        GameObject recipeInstance = Instantiate(recipePrefab,recipesPanel.transform, false);
        recipeInstance.GetComponent<RecipeUI>().SetRecipe(recipe);

        Transform ingredientsPanel = recipeInstance.transform.Find("RecipeInfoPanel/IngredientList");
        print("panel: "+ ingredientsPanel);
        foreach(RecipeIngredient ingredient in recipe.ingredients)
        {   
            GameObject ingredientInstance =  Instantiate(ingredientPrefab,ingredientsPanel,false);
            ingredientInstance.GetComponent<IngredientUI>().SetIngredient(ingredient);
        }
    }

    public void ShowTools() => ShowRecipes(RecipeType.Tool);
    public void ShowPlaceables() => ShowRecipes(RecipeType.Placeable);
}
