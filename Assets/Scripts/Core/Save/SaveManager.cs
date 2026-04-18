using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    [SerializeField] private Transform worldObjects;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        if (File.Exists(Application.persistentDataPath + "/save.json"))
            StartCoroutine(LoadGameCR());
    }

    // =====================================================
    // SAVE
    // =====================================================
    public void SaveGame()
    {
        SaveData data = new();

        // ---------------- PLAYER ----------------
        data.playerData = new SaveData.PlayerData();

        PlayerAttributes player = PlayerController.instance.GetPlayerAttributes();

        data.playerData.playerHealth = player.GetCurrentHealth();
        data.playerData.playerHunger = player.GetCurrentHunger();
        data.playerData.playerStamina = player.GetCurrentStamina();
        data.playerData.playerPosition = player.transform.position;
        data.playerData.playerRotation = player.transform.rotation;
        data.playerData.cameraRotation = CameraController.instance.GetCurrentRotation();

        // ---------------- INVENTORY ----------------
        data.inventoryItems = new List<SaveData.InventoryItemData>();

        InventoryController inventory = InventoryController.instance;

        for (int i = 0; i < inventory.GetInventoryItems().Length; i++)
        {
            SaveData.InventoryItemData itemData = new();

            ItemStack instance = inventory.GetInventoryItems()[i];

            itemData.inventoryIndex = i;

            if (instance != null)
            {
                itemData.id = instance.id;
                itemData.currentHealth = instance.currentHealth;
            }
            else
            {
                itemData.id = "-1";
                itemData.currentHealth = 0;
            }

            data.inventoryItems.Add(itemData);
        }

        // ---------------- WORLD OBJECTS ----------------
        data.worldObjects = new List<SaveData.WorldObjectData>();

        for (int i = 0; i < worldObjects.childCount; i++)
        {
            Transform worldObject = worldObjects.GetChild(i);

            SaveData.WorldObjectData objectData = new();

            objectData.position = worldObject.position;
            objectData.rotation = worldObject.rotation;

            // ITEM EN EL MUNDO
            if (worldObject.TryGetComponent(out ItemBehaviour itemBehaviour))
            {
                ItemStack instance = itemBehaviour.GetItemStack();

                objectData.id = instance.id;
                objectData.type = "Item";
                objectData.currentHealth = instance.currentHealth;
            }
            
            // HARVESTABLE
            else if (worldObject.TryGetComponent(out HarvestableObject harvestable))
            {
                objectData.id = harvestable.GetObjectName();
                objectData.type = "Harvestable";
                objectData.currentHealth = harvestable.GetCurrentHealth();
            }

            data.worldObjects.Add(objectData);
        }

        // ---------------- WRITE ----------------
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.persistentDataPath + "/save.json", json);
    }

    // =====================================================
    // LOAD
    // =====================================================
    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/save.json";

        if (!File.Exists(path))
        {
            Debug.LogWarning("No hay partida guardada");
            return;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // ---------------- PLAYER ----------------
        PlayerController player = PlayerController.instance;

        player.InitializePlayer(
            data.playerData.playerPosition,
            data.playerData.playerRotation,
            data.playerData.playerHealth,
            data.playerData.playerHunger,
            data.playerData.playerStamina
        );

        CameraController.instance.SetCurrentRotation(data.playerData.cameraRotation);

        // ---------------- INVENTORY ----------------
        InventoryController inventory = InventoryController.instance;

        for (int i = 0; i < inventory.GetInventoryItems().Length; i++)
        {
            var itemData = data.inventoryItems[i];

            if (itemData.id == "-1")
            {
                inventory.SetItem(i, null);
                continue;
            }

            ItemStack instance = new ItemStack
            {
                id = itemData.id,
                currentHealth = itemData.currentHealth
            };

            inventory.SetItem(i, instance);
        }

        // ---------------- WORLD OBJECTS ----------------
        for (int i = 0; i < data.worldObjects.Count; i++)
        {
            var objectData = data.worldObjects[i];

            GameObject prefab = ObjectsPrefabs.instance.GetPrefabByName(
                objectData.type,
                objectData.id
            );

            GameObject obj = Instantiate(prefab, worldObjects);

            obj.transform.position = objectData.position;
            obj.transform.rotation = objectData.rotation;

            if (obj.TryGetComponent(out IObjectHealth health))
            {
                health.SetCurrentHealth(objectData.currentHealth);
            }
        }
    }

    private IEnumerator LoadGameCR()
    {
        yield return null;
        LoadGame();
    }
}