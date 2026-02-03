using Unity.VisualScripting;
using UnityEngine;

public abstract class ItemBehaviour : MonoBehaviour
{   
    protected ItemData itemData;
    protected virtual void Awake()
    {
        itemData = GetComponent<ItemData>();
    }
    public abstract void Use();
}
