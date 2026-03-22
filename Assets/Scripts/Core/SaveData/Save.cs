using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public Vector3 playerPosition;
    public float playerHealth;
    public List<ItemDataSerializable> inventoryItems = new();
    public List<HarvestableSave> harvestables = new();
}

[Serializable]
public class ItemDataSerializable
{
    public string prefabName;
    public int currentHealth; // para herramientas
}

[Serializable]
public class HarvestableSave
{
    public string prefabName;
    public Vector3 position;
    public float currentHealth;
}