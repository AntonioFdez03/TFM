using UnityEngine;

[CreateAssetMenu(menuName = "Furnace/Fuel Data")]
public class FuelData : ScriptableObject
{
    public ItemData fuelItem;
    public int energy;
}

