using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyDataBase : MonoBehaviour
{   
    public static KeyDataBase instance;
    [SerializeField] private List<KeyData> keys = new();

    private Dictionary<string, Sprite> cache;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void BuildCache()
    {
        cache = new();

        foreach (var key in keys)
        {
            if (!cache.ContainsKey(key.id))
                cache.Add(key.id, key.icon);
        }
    }

    public Sprite GetIcon(string id)
    {
        if (cache == null)
            BuildCache();

        if (cache.TryGetValue(id, out Sprite icon))
            return icon;

        return null;
    }
}