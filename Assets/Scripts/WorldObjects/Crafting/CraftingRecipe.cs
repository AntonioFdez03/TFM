using System.Collections.Generic;
using UnityEngine;

public enum RecipeType {Tool, Weapon, Consumable}

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public RecipeType recipeType;
    public List<RecipeIngredient> ingredients;
    public ItemData recipeData;
}

[System.Serializable]
public class RecipeIngredient
{
    public ItemData ingredientData;
    public int ingredientAmount;
}