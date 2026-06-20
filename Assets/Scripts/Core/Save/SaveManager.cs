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
        if (MainMenuManager.GameContinued())
        {
            print("Game continued");
            StartCoroutine(LoadGameCR());
        }
        else
        {
            print("No continued");
        }
    }

    // =====================================================
    // SAVE
    // =====================================================
    public bool SaveGame()
    {   
        print("Guardando");
        // ---------------- PLAYER ----------------
        SaveData data = new();

        data.playerData = new SaveData.PlayerData();
        PlayerAttributes player = PlayerController.instance.GetPlayerAttributes();

        data.playerData.playerHealth = player.GetCurrentHealth();
        data.playerData.playerHunger = player.GetCurrentHunger();
        data.playerData.playerStamina = player.GetCurrentStamina();
        data.playerData.playerSanity = player.GetCurrentSanity();
        data.playerData.playerInLight = player.InLight();
        data.playerData.playerPosition = player.transform.position;
        data.playerData.playerRotation = player.transform.rotation;
        data.playerData.cameraRotation = CameraController.instance.GetCurrentRotation();
        data.playerData.selectedHotBarIndex = HotBarController.instance.GetSelectedIndex();

        // ---------------- INVENTORY ----------------
        data.inventoryItems = new List<SaveData.InventoryItemData>();

        InventoryController inventory = InventoryController.instance;

        for (int i = 0; i < inventory.GetInventoryItems().Length; i++)
        {
            SaveData.InventoryItemData itemData = new();

            ItemStack inventoryItem = inventory.GetInventoryItems()[i];

            itemData.inventoryIndex = i;

            if (inventoryItem != null)
            {
                itemData.id = inventoryItem.id;
                itemData.currentHealth = inventoryItem.currentHealth;
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

            SaveData.WorldObjectData objectData = new()
            {
                position = worldObject.position,
                rotation = worldObject.rotation
            };

            // ITEM EN EL MUNDO
            if (worldObject.TryGetComponent(out ItemBehaviour itemBehaviour))
            {
                ItemStack instance = itemBehaviour.GetItemStack();

                objectData.id = instance.id;
                print("Guardando item: " + objectData.id);
                objectData.type = "Item";
                objectData.currentHealth = instance.currentHealth;

                if(worldObject.TryGetComponent(out StorageController storage))
                {   
                    print("Guardando cofre");
                    objectData.storageItems = new();

                    for(int j = 0; j < storage.GetStorageSize(); j++)
                    {   
                        SaveData.InventoryItemData itemData = new();
                        ItemStack storageItem = storage.GetItem(j);
                        itemData.inventoryIndex = j;

                        if (storageItem != null)
                        {
                            print("Datos guardados");
                            itemData.id = storageItem.id;
                            itemData.currentHealth = storageItem.currentHealth;
                        }
                        else
                        {   
                            print("Datos NO guardados");
                            itemData.id = "-1";
                            itemData.currentHealth = 0;
                        }

                        objectData.storageItems.Add(itemData);
                    }
                }else if(worldObject.TryGetComponent(out FurnaceController furnace))
                {   
                    print("Gurdando horno");
                    // Inputs
                    objectData.furnaceInputItems = new();

                    for(int j = 0; j < furnace.GetInputItems().Length; j++)
                    {   
                        SaveData.InventoryItemData itemData = new();
                        ItemStack furnaceItem = furnace.GetInputItems()[j];
                        itemData.inventoryIndex = j;

                        if (furnaceItem != null)
                        {   
                            print("Hay input");
                            itemData.id = furnaceItem.id;
                            itemData.currentHealth = furnaceItem.currentHealth;
                        }
                        else
                        {
                            itemData.id = "-1";
                            itemData.currentHealth = 0;
                        }

                        objectData.furnaceInputItems.Add(itemData);
                    }

                    // Outputs 
                    objectData.furnaceOutputItems = new();

                    for(int j = 0; j < furnace.GetOutputItems().Length; j++)
                    {   
                        SaveData.InventoryItemData itemData = new();
                        ItemStack furnaceItem = furnace.GetOutputItems()[j];
                        itemData.inventoryIndex = j;

                        if (furnaceItem != null)
                        {
                            itemData.id = furnaceItem.id;
                            itemData.currentHealth = furnaceItem.currentHealth;
                        }
                        else
                        {
                            itemData.id = "-1";
                            itemData.currentHealth = 0;
                        }

                        objectData.furnaceOutputItems.Add(itemData);
                    }

                    // Fuel
                    SaveData.InventoryItemData fuelData = new();
                    ItemStack furnaceFuelItem = furnace.GetFuelItem();
                    fuelData.inventoryIndex = i;

                    if (furnaceFuelItem != null)
                    {
                        fuelData.id = furnaceFuelItem.id;
                        fuelData.currentHealth = furnaceFuelItem.currentHealth;
                    }
                    else
                    {
                        fuelData.id = "-1";
                        fuelData.currentHealth = 0;
                    }

                    objectData.furnaceFuelItem = fuelData;
                }
            }
            
            // HARVESTABLE
            else if (worldObject.TryGetComponent(out HarvestableObject harvestable))
            {
                objectData.id = harvestable.GetData().id;
                objectData.type = "Harvestable";
                objectData.currentHealth = harvestable.GetCurrentHealth();
            }

            print("Item añadido con id: " + objectData.id);
            data.worldObjects.Add(objectData);
        }

        data.dayData = new SaveData.DayData
        {
            currentDay = DayCycleController.instance.GetCurrentDay(),
            currentHour = DayCycleController.instance.GetCurrentHour()
        };

        // ---------------- WRITE ----------------
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.persistentDataPath + "/save.json", json);
        return true;
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

        ClearWorld();

        print("Cargando datos..");
        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // ---------------- PLAYER ----------------
        PlayerController player = PlayerController.instance;

        player.InitializePlayer(
            data.playerData.playerPosition,
            data.playerData.playerRotation,
            data.playerData.playerHealth,
            data.playerData.playerHunger,
            data.playerData.playerStamina,
            data.playerData.playerSanity,
            data.playerData.playerInLight
        );

        CameraController.instance.SetCurrentRotation(data.playerData.cameraRotation);
        HotBarController.instance.MoveSelectorFrame(data.playerData.selectedHotBarIndex);

        // ---------------- INVENTORY ----------------
        InventoryController inventory = InventoryController.instance;

        for (int i = 0; i < inventory.GetInventorySize(); i++)
        {
            var itemData = data.inventoryItems[i];

            if (itemData.id == "-1")
            {
                inventory.SetItem(i, null);
                continue;
            }

            ItemStack instance = new()
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

            GameObject prefab = ObjectsPrefabs.instance.GetPrefabByID(
                objectData.type,
                objectData.id
            );

            GameObject obj = Instantiate(prefab, worldObjects);

            obj.transform.SetPositionAndRotation(objectData.position, objectData.rotation);
            
            if (obj.TryGetComponent(out IObjectHealth health))
            {
                health.SetCurrentHealth(objectData.currentHealth);
            }

            if(obj.TryGetComponent(out StorageController storage))
            {   
                // Load storage items
                for(int j = 0; j < storage.GetStorageSize(); j++)
                {
                    var itemData = objectData.storageItems[j];

                    if (itemData.id == "-1")
                    {
                        storage.SetItem(j, null);
                        continue;
                    }

                    ItemStack instance = new()
                    {
                        id = itemData.id,
                        currentHealth = itemData.currentHealth
                    };

                    storage.SetItem(j, instance);
                }
            }else if(obj.TryGetComponent(out FurnaceController furnace))
            {   
                // Load inputs
                for(int j = 0; j < furnace.GetFurnaceSize(); j++)
                {
                    var itemData = objectData.furnaceInputItems[j];

                    if (itemData.id == "-1")
                    {
                        furnace.AddInput(j, null);
                        continue;
                    }

                    ItemStack instance = new()
                    {
                        id = itemData.id,
                        currentHealth = itemData.currentHealth
                    };

                    furnace.AddInput(j, instance);
                }

                // Load outputs
                for(int j = 0; j < furnace.GetFurnaceSize(); j++)
                {
                    var itemData = objectData.furnaceOutputItems[j];

                    if (itemData.id == "-1")
                    {
                        furnace.AddOutput(j, null);
                        continue;
                    }

                    ItemStack instance = new()
                    {
                        id = itemData.id,
                        currentHealth = itemData.currentHealth
                    };

                    furnace.AddOutput(j, instance);
                }

                // Load fuel
                var fuelData = objectData.furnaceFuelItem;

                    if (fuelData.id == "-1")
                    {
                        furnace.AddFuel(null);
                        continue;
                    }

                    ItemStack fuelInstance = new()
                    {
                        id = fuelData.id,
                        currentHealth = fuelData.currentHealth
                    };

                    print("Añadiendo fuel: " + fuelInstance);
                    if(furnace.AddFuel(fuelInstance))
                        print("Añadido");
                    

                    furnace.SetCurrentTimer(objectData.furnaceTimer);
            }
        }
        // ---------------- DAY CYCLE ----------------
        DayCycleController dayCycle = DayCycleController.instance;
        dayCycle.Initialize(data.dayData.currentDay, data.dayData.currentHour);
        print("Fin de la carga");
    }

    private void ClearWorld()
    {
        for (int i = 0; i < worldObjects.childCount; i++)
        {   
            Destroy(worldObjects.GetChild(i).gameObject);
        }
    }

    private IEnumerator LoadGameCR()
    {
        yield return null;
        LoadGame();
    }
}