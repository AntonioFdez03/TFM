using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ItemBehaviour : MonoBehaviour
{   
    protected ItemData itemData;
    protected float useCooldown = 0.5f;
    protected bool canUse = true;
    protected virtual void Awake()
    {
        itemData = GetComponent<ItemData>();
    }
    public abstract void Use();

    IEnumerator UseCooldown()
    {
        yield return new WaitForSeconds(useCooldown);
        print("Herramienta usada");
    }

    public ItemData GetItemData() => itemData;
}
