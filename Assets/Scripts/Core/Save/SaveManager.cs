using System.Collections;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SaveManager : MonoBehaviour
{   
    public static SaveManager instance;

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
        
        //PLAYER
        data.playerData = new SaveData.PlayerData();
        PlayerAttributes player = PlayerController.instance.GetPlayerAttributes();

        data.playerData.playerHealth = player.GetCurrentHealth();
        data.playerData.playerHunger = player.GetCurrentHunger();
        data.playerData.playerStamina = player.GetCurrentStamina();
        data.playerData.playerPosition = player.transform.position;
        data.playerData.playerRotation = player.transform.rotation;
        data.playerData.cameraRotation = CameraController.instance.GetCurrentRotation();

        // INVENTORY
        data.inventoryItems = new System.Collections.Generic.List<SaveData.InventoryItemData>();

        InventoryController inventory = InventoryController.instance;
        GameObject item;

        for(int i = 0; i < inventory.GetInventoryItems().Length; i++)
        {
            SaveData.InventoryItemData itemData = new SaveData.InventoryItemData();
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
                itemData.itemHealth = -1;
            }
            data.inventoryItems.Add(itemData);
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
            {
                print("Añadiendo null ...");
                newItem = null;
            }
            else
            {   
                print("Añadiendo item ...");
                GameObject itemPrefab = ItemsPrefabs.instance.GetPrefabByName(itemData.itemName);
                if(itemPrefab.TryGetComponent(out EquipmentBehaviour equipmentBehaviour))
                    equipmentBehaviour.SetCurrentHealth(itemData.itemHealth);

                newItem = Instantiate(itemPrefab);
            }
            inventory.SetItem(i,newItem);
        }
    }     

    private IEnumerator LoadGameCR()
    {
        yield return null;
        LoadGame();
    }
}