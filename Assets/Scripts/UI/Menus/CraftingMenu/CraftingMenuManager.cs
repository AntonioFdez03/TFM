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
        ShowRecipes(ItemType.None);
    }

    void OnEnable()
    {   
        EnableButtons();
        CleanRecipes();
        ShowRecipes(ItemType.None);

        if(CraftingController.instance == null)
            return;
            
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

    public void ShowRecipes(ItemType recipeType)
    {
        CleanRecipes();
        if(CraftingController.instance == null)
            return;

        foreach(CraftingRecipe recipe in CraftingController.instance.GetAllRecipeList())
        {
            if((recipeType == ItemType.None || recipe.recipeType == recipeType) && (recipe.stationType == CraftingController.instance.GetStationType() || recipe.stationType == CraftingStationType.None))
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
        foreach(RecipeIngredient ingredient in recipe.ingredients)
        {   
            GameObject ingredientInstance =  Instantiate(ingredientPrefab,ingredientsPanel,false);
            ingredientInstance.GetComponent<IngredientUI>().SetIngredient(ingredient);
        }
    }

    public void ToolsButtonPressed() {
        EnableButtons();
        toolsButton.interactable = false;
        ShowRecipes(ItemType.Tool);
    }
    public void PlaceablesButtonPressed() 
    {
        EnableButtons();
        placeablesButton.interactable = false;
        ShowRecipes(ItemType.Placeable);
    }

    private void EnableButtons()
    {
        toolsButton.interactable = true;
        placeablesButton.interactable = true;
    }
}
