using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{   
    public static InventoryController inventoryInstance;
    [Header("References")]
    [SerializeField] Transform itemsLayer; 
    [SerializeField] InventoryUI inventoryUI;

    private int inventoryMax = 28;
    private GameObject[] items;

    void Awake()
    {
        if(inventoryInstance != null && inventoryInstance != this)
        {
            Destroy(gameObject);
            return;
        }
        inventoryInstance = this;
    }
    
    void Start()
    {
        items = new GameObject[inventoryMax];
    }

    public GameObject[] GetInventoryItems() => items;

    public void AddItem(GameObject item)
    {
        print("Item añadido: "+item);
        // Buscamos el primer hueco vacío (null)
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                item.transform.SetParent(this.transform);
                item.SetActive(false);
                
                if(inventoryUI != null) inventoryUI.UpdateUI();
                return;
            }
        }
        Debug.Log("Inventario lleno");
    }

    public void DropItem(int index)
    {
        // Verificamos que el índice sea válido y que haya algo en ese slot
        if (index >= 0 && index < items.Length && items[index] != null)
        {
            GameObject itemToDrop = items[index];
            
            // Vaciamos el slot
            items[index] = null; 

            itemToDrop.SetActive(true);
            itemToDrop.transform.SetParent(itemsLayer);
            itemToDrop.transform.rotation = Random.rotation;
            Rigidbody rb = itemToDrop.GetComponentInChildren<Rigidbody>();
            Vector3 dropForce = Vector3.forward * 5f + Vector3.down * 4f;
            rb.AddForce(dropForce,ForceMode.Impulse);

            if(inventoryUI != null) inventoryUI.UpdateUI();
        }
    }

    public void SwapItems(int originIndex, int targetIndex)
    {
        if (originIndex == targetIndex) return;

        //Intercambio
        (items[targetIndex], items[originIndex]) = (items[originIndex], items[targetIndex]);
        inventoryUI.UpdateUI();
    }
}