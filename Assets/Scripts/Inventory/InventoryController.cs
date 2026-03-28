using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InventoryController : MonoBehaviour
{   
    public static InventoryController instance;
    public static Action OnInventoryChanged;

    [SerializeField] Transform itemsParent; 
    [SerializeField] Transform handSlot;

    private int inventoryMax = 28;
    private int hotBarSize = 7;
    private int inventoryGridSize = 21;

    private GameObject[] items;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        items = new GameObject[inventoryMax];
    }
    
    public void SetInventoryItems(GameObject[] newItems) => items = newItems;
    public GameObject[] GetInventoryItems() => items;
    public int GetHotBarSize() => hotBarSize;
    public int GetInventoryGridSize() => inventoryGridSize;
    public Transform GetItemsParent() => itemsParent;

    public void AddItem(GameObject item)
    {   
        if(item == null)
            return;

        // Buscamos el primer hueco vacío (null)
        for (int i = 0; i < items.Length; i++)
        {   
            if (items[i] == null)
            {   
                items[i] = item;
                item.transform.SetParent(transform);
                item.SetActive(false);
                
                OnInventoryChanged?.Invoke();
                HotBarController.instance.UpdateHotBarUI();
                return;
            }
        }
    }

    public void SetItem(int index, GameObject item)
    {
        if(index >= 0 && index < inventoryMax && items[index] == null && item != null)
        {
            items[index] = item;
            OnInventoryChanged?.Invoke();
            HotBarController.instance.UpdateHotBarUI();
        }
    }

    public void RemoveItem(GameObject item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == item)
            {   
                items[i] = null;
                Destroy(item);
                OnInventoryChanged?.Invoke();
                HotBarController.instance.UpdateHotBarUI();
                return;
            }
        }
    }

    public void DropItem(int index)
    {
        if (index >= 0 && index < items.Length && items[index] != null)
        {
            GameObject itemToDrop = items[index];
            
            // Vaciamos el slot
            items[index] = null; 
            itemToDrop.SetActive(true);

            //Posicion, giro y escala al soltarlo
            itemToDrop.transform.position = handSlot.transform.position;
            Transform player = PlayerController.instance.transform;
            itemToDrop.transform.rotation = player.rotation * Quaternion.Euler(0f, 0f, 90f);

            Rigidbody rb = itemToDrop.GetComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.isKinematic = false;

            itemToDrop.transform.SetParent(itemsParent, true);
            itemToDrop.transform.localScale = Vector3.one;

            Vector3 dropForce = CameraController.instance.transform.forward * 50f + CameraController.instance.transform.up * 40f;
            rb.AddForce(dropForce,ForceMode.Impulse);

            OnInventoryChanged?.Invoke();
            HotBarController.instance.UpdateHotBarUI();
        }
    }

    public void SwapItems(int originIndex, int targetIndex)
    {
        if (originIndex == targetIndex) return;

        (items[targetIndex], items[originIndex]) = (items[originIndex], items[targetIndex]);
        OnInventoryChanged?.Invoke();
        HotBarController.instance.UpdateHotBarUI();
    }

    public List<GameObject> GetItemsByName(string name)
    {
        List<GameObject> result = new List<GameObject>();

        foreach (GameObject item in items)
        {
            if (item == null) continue;

            ItemData data = item.GetComponent<ItemData>();
            if (data != null && data.GetItemName() == name)
            {
                result.Add(item);
            }
        }

        return result;
    }
}