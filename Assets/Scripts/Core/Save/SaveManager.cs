using System.Collections;
using System.IO;
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
        if(MainMenuManager.instance.GameDataFound())
            StartCoroutine(LoadGameCR());
    }

    public void SaveGame()
    {
        SaveData data = new();
        
        data.playerData = new SaveData.PlayerData();
        PlayerAttributes player = PlayerController.instance.GetPlayerAttributes();

        data.playerData.playerHealth = player.GetCurrentHealth();
        data.playerData.playerHunger = player.GetCurrentHunger();
        data.playerData.playerStamina = player.GetCurrentStamina();
        data.playerData.playerPosition = player.transform.position;
        data.playerData.playerRotation = player.transform.rotation;
        data.playerData.cameraRotation = CameraController.instance.GetCurrentRotation();

        // ======================
        // INVENTORY
        // ======================
        /*
        data.inventoryItems = new System.Collections.Generic.List<SaveData.InventoryItemData>();

        InventoryController inventory = InventoryController.instance;

        foreach (var item in inventory.GetItems())
        {
            SaveData.InventoryItemData itemData = new SaveData.InventoryItemData();

            itemData.itemID = item.itemID;
            itemData.currentHealth = item.currentHealth;

            data.inventoryItems.Add(itemData);
        }

        // ======================
        // WORLD OBJECTS
        // ======================
        data.worldObjects = new System.Collections.Generic.List<SaveData.WorldObjectData>();

        var worldObjects = FindObjectsOfType<MonoBehaviour>();

        foreach (var obj in worldObjects)
        {
            if (obj is PlaceableBehaviour placeable)
            {
                SaveData.WorldObjectData objData = new SaveData.WorldObjectData();

                objData.prefabID = placeable.prefabID;
                objData.position = obj.transform.position;
                objData.rotation = obj.transform.rotation;

                if (obj is IHasHealth hasHealth)
                {
                    objData.currentHealth = hasHealth.CurrentHealth;
                }

                data.worldObjects.Add(objData);
            }
        }
    */

        // ======================
        // GUARDAR A ARCHIVO
        // ======================
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
        Debug.Log("Juego cargado");
    }

    private IEnumerator LoadGameCR()
    {
        yield return null;
        LoadGame();
    }
}