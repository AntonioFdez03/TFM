using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    [Serializable]
    public class PlayerData
    {
        public float playerHealth;
        public float playerHunger;
        public float playerStamina;
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        public float cameraRotation;
    }
    public PlayerData playerData;

    // INVENTORY
    [Serializable]
    public class InventoryItemData
    {   
        public int inventoryIndex;
        public string itemName;
        public float itemHealth;
    }
    public List<InventoryItemData> inventoryItems;

    // WORLD OBJECTS
    [Serializable]
    public class WorldObjectData
    {
        public string name;
        public string type;
        public Vector3 position;
        public Quaternion rotation;
        public float currentHealth;
    }

    public List<WorldObjectData> worldObjects;
}