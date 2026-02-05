using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ItemBehaviour : MonoBehaviour
{  
    protected ItemData itemData;
    protected Camera playerCamera;
    protected float useCooldown = 1f;
    protected bool canUse = true;

    protected virtual void Awake()
    {
        itemData = GetComponent<ItemData>();
        playerCamera = Camera.main;
    }
    public abstract void Use();

    protected IEnumerator UseCooldown()
    {
        yield return new WaitForSeconds(useCooldown);
        canUse = true;
    }

    public ItemData GetItemData() => itemData;
}
