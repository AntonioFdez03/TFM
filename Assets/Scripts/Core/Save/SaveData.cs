using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        public float playerSanity;
        public bool playerInLight;
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        public float cameraRotation;
        public int selectedHotBarIndex;
    }
    public PlayerData playerData;

    // INVENTORY
    [Serializable]
    public class InventoryItemData
    {   
        public int inventoryIndex;
        public string id;
        public float currentHealth;
    }
    public List<InventoryItemData> inventoryItems;

    // WORLD OBJECTS
    [Serializable]
    public class WorldObjectData
    {
        public string id;
        public string type;
        public Vector3 position;
        public Quaternion rotation;
        public float currentHealth;

        // Storage
        public List<InventoryItemData> storageItems;

        // Furnace
        public List<InventoryItemData> furnaceInputItems;
        public List<InventoryItemData> furnaceOutputItems;
        public InventoryItemData furnaceFuelItem;
        public float furnaceTimer;
    }

    public List<WorldObjectData> worldObjects;

    // Day cycle
    [Serializable]
    public class DayData
    {
        public int currentDay;
        public float currentHour;
    }

    public DayData dayData;

    [Serializable] 
    public class ObjectiveData
    {
        public ObjectiveType currentObjective;
        public int objectiveDaysSurvived;

    }

    public ObjectiveData objectiveData;

    [Serializable]
    public class StatsData
    {
        public int daysSurvived;
        public int treesChopped;
        public int rocksDestroyed;
        public int bushesRecolected;
        public int animalsHunted;
        public int itemsCrafted;
        public int itemsPlaced;

    }

    public StatsData statsData;
}