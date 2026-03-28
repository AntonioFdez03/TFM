using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SaveManager : MonoBehaviour
{   
    public static SaveManager instance;
    [SerializeField] private Transform worldObjects;


    void Awake()
    {
        if(instance != null && instance != this)
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

    public void SaveGame()
    {
        SaveData data = new();
        
        // PLAYER
        data.playerData = new SaveData.PlayerData();
        PlayerAttributes player = PlayerController.instance.GetPlayerAttributes();

        data.playerData.playerHealth = player.GetCurrentHealth();
        data.playerData.playerHunger = player.GetCurrentHunger();
        data.playerData.playerStamina = player.GetCurrentStamina();
        data.playerData.playerPosition = player.transform.position;
        data.playerData.playerRotation = player.transform.rotation;
        data.playerData.cameraRotation = CameraController.instance.GetCurrentRotation();

        // INVENTORY
        data.inventoryItems = new List<SaveData.InventoryItemData>();

        InventoryController inventory = InventoryController.instance;
        GameObject item;

        for(int i = 0; i < inventory.GetInventoryItems().Length; i++)
        {
            SaveData.InventoryItemData itemData = new();
            item = inventory.GetInventoryItems()[i];

            if(item != null)
            {   
                itemData.inventoryIndex = i;
                itemData.itemName = item.GetComponent<ItemData>().GetItemName();
                itemData.itemHealth = item.GetComponent<ItemBehaviour>().GetCurrentHealth();
            }
            else
            {   
                itemData.itemName = "-1";
                itemData.itemHealth = 0;
            }
            data.inventoryItems.Add(itemData);
        }

        // WORLD OBJECTS
        data.worldObjects = new List<SaveData.WorldObjectData>();
        Transform worldObject;

        for(int i = 0; i < worldObjects.childCount ; i++)
        {
            SaveData.WorldObjectData objectData = new();
            worldObject = worldObjects.GetChild(i);

            objectData.position = worldObject.position;
            objectData.rotation = worldObject.rotation;

            if(worldObjects.GetChild(i).TryGetComponent(out ItemBehaviour itemBehaviour))
            {
                objectData.name = itemBehaviour.GetItemData().GetItemName();
                objectData.type = "Item";
                objectData.currentHealth = itemBehaviour.GetCurrentHealth();
            }else if(worldObjects.GetChild(i).TryGetComponent(out HarvestableObject harvestable))
            {
                objectData.name = harvestable.GetObjectName();
                objectData.type = "Harvestable";
                objectData.currentHealth = harvestable.GetCurrentHealth();
            }

            data.worldObjects.Add(objectData);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.persistentDataPath + "/save.json", json);
        print(Application.persistentDataPath);
    }

    public void LoadGame()
    {   
        string path = Application.persistentDataPath + "/save.json";

        if (!File.Exists(path))
        {
            Debug.LogWarning("No hay partida guardada");
            return;
        }

        // Leer archivo
        string json = File.ReadAllText(path);
        // Convertir JSON a objeto
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // PLAYER

        PlayerController player = PlayerController.instance;
        player.InitializePlayer(
            data.playerData.playerPosition,
            data.playerData.playerRotation,
            data.playerData.playerHealth,
            data.playerData.playerHunger,
            data.playerData.playerStamina
        );

        CameraController.instance.SetCurrentRotation(data.playerData.cameraRotation);

        InventoryController inventory = InventoryController.instance;
        GameObject newItem;

        for (int i = 0; i < inventory.GetInventoryItems().Length; i++)
        {   
            var itemData = data.inventoryItems[i];
            
            if(itemData.itemName == "-1")
                newItem = null;
            else
            {   
                GameObject itemPrefab = ObjectsPrefabs.instance.GetPrefabByName("Item", itemData.itemName);
                if(itemPrefab.TryGetComponent(out EquipmentBehaviour equipmentBehaviour))
                    equipmentBehaviour.SetCurrentHealth(itemData.itemHealth);

                newItem = Instantiate(itemPrefab);
            }
            inventory.SetItem(i,newItem);
        }
        
        GameObject newObject;
        for(int i = 0; i < data.worldObjects.Count ; i++)
        {   
            var objectData = data.worldObjects[i];
            print("Instanciando: " + objectData.name);
            GameObject objectPrefab = ObjectsPrefabs.instance.GetPrefabByName(objectData.type, objectData.name);
            objectPrefab.GetComponent<IObjectHealth>().SetCurrentHealth(objectData.currentHealth);
            newObject = Instantiate(objectPrefab,InventoryController.instance.GetItemsParent());
            newObject.transform.position = objectData.position;
            newObject.transform.rotation = objectData.rotation;
        }
        
    }     

    private IEnumerator LoadGameCR()
    {
        yield return null;
        LoadGame();
    }
}