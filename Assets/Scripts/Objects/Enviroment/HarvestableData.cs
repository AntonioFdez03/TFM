using UnityEngine;

[CreateAssetMenu(menuName = "Enviroment/Harvestable Data")]
public class HarvestableData : ScriptableObject
{
    public string id;
    public float maxHealth;
    public GameObject prefab;
}