using System.Collections.Generic;
using UnityEngine;

public enum RecipeType {None, Tool, Weapon, Consumable, Placeable}
public enum CraftingStationType { None, Workbench, Anvil}

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public RecipeType recipeType;
    public CraftingStationType stationType;
    public List<RecipeIngredient> ingredients;
    public GameObject recipeItem;

}

[System.Serializable]
public class RecipeIngredient
{
    public ItemData ingredientData;
    public int ingredientAmount;
}