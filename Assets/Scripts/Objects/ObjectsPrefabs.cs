using System.Collections.Generic;
using UnityEngine;

public class ObjectsPrefabs : MonoBehaviour
{   
    public static ObjectsPrefabs instance;
    public List<GameObject> itemsPrefabs;
    public List<GameObject> harvestablesPrefabs;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public GameObject GetPrefabByID(string type, string id)
    {   
        print("Buscando item: " + id);
        string cleanName = id.Trim().ToLower();
        List<GameObject> prefabList;

        if(type == "Item")
            prefabList = itemsPrefabs;
        else
            prefabList = harvestablesPrefabs;

        foreach (GameObject prefab in prefabList)
        {
            if (prefab == null) continue;

            string prefabCleanName = prefab.name.Trim().ToLower();

            if (prefabCleanName == cleanName)
                return prefab;
        }

        Debug.LogError("Prefab no encontrado: " + id);
        return null;
    }
}