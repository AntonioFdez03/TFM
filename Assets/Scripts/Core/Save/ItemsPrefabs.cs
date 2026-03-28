using System.Collections.Generic;
using UnityEngine;

public class ItemsPrefabs : MonoBehaviour
{   
    public static ItemsPrefabs instance;
    public List<GameObject> prefabs;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public GameObject GetPrefabByName(string prefabName)
    {
        string cleanName = prefabName.Trim().ToLower();

        foreach (GameObject prefab in prefabs)
        {
            if (prefab == null) continue;

            string prefabCleanName = prefab.name.Trim().ToLower();

            if (prefabCleanName == cleanName)
                return prefab;
        }

        Debug.LogError("Prefab no encontrado: " + prefabName);
        return null;
    }
}