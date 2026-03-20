using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Unity.VisualScripting;
using TMPro;
using UnityEditor;

public class CraftingMenuManager : MonoBehaviour
{   
    [SerializeField] private TMP_Text stationType;
    [SerializeField] private GameObject recipesPanel;
    [SerializeField] private GameObject recipePrefab;
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private Button toolsButton;
    [SerializeField] private Button placeablesButton;

    void Start()
    {   
        CleanRecipes();
        ShowRecipes(RecipeType.None);
    }

    void OnEnable()
    {   
        EnableButtons();
        CleanRecipes();
        ShowRecipes(RecipeType.None);

        if(CraftingController.instance.GetStationType() != CraftingStationType.None)
            stationType.text = "(" + CraftingController.instance.GetStationType().ToString().ToUpper() + ")";
        else
            stationType.text = "";
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
            if((recipeType == RecipeType.None || recipe.recipeType == recipeType) && (recipe.stationType == CraftingController.instance.GetStationType() || recipe.stationType == CraftingStationType.None))
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

    public void ToolsButtonPressed() {
        EnableButtons();
        toolsButton.interactable = false;
        ShowRecipes(RecipeType.Tool);
    }
    public void PlaceablesButtonPressed() 
    {
        EnableButtons();
        placeablesButton.interactable = false;
        ShowRecipes(RecipeType.Placeable);
    }

    private void EnableButtons()
    {
        toolsButton.interactable = true;
        placeablesButton.interactable = true;
    }
}
