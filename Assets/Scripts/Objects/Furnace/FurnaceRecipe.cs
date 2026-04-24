using UnityEngine;

[CreateAssetMenu(fileName = "NewFurnaceRecipe", menuName = "Furnace/Furnace Recipe")]
public class FurnaceRecipe : ScriptableObject
{
    public ItemData inputItem;

    public ItemData resultItem;

    public float cookTime = 5f;
}