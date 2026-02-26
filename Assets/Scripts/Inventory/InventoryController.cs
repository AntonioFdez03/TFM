using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InventoryController : MonoBehaviour
{   
    public static InventoryController inventoryInstance;
    [Header("References")]
    [SerializeField] Transform itemsLayer; 
    [SerializeField] Transform handSlot;

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
        // Buscamos el primer hueco vacío (null)
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                item.transform.SetParent(this.transform);
                item.SetActive(false);
                
                InventoryUI.instance.UpdateUI();
                return;
            }
        }
        Debug.Log("Inventario lleno");
    }

    public void RemoveItem(GameObject item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == item)
            {
                items[i] = null;
                InventoryUI.instance.UpdateUI();
                return;
            }
        }
    }
    public void DropItem(int index)
    {
        print("DROP");
        // Verificamos que el índice sea válido y que haya algo en ese slot
        if (index >= 0 && index < items.Length && items[index] != null)
        {
            GameObject itemToDrop = items[index];
            
            // Vaciamos el slot
            items[index] = null; 

            itemToDrop.SetActive(true);

            //Posicion, giro y escala al soltarlo
            itemToDrop.transform.position = handSlot.transform.position;
            Transform player = PlayerController.playerInstance.transform;
            itemToDrop.transform.rotation = player.rotation * Quaternion.Euler(0f, 0f, 90f);

            Rigidbody rb = itemToDrop.GetComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.isKinematic = false;

            itemToDrop.transform.SetParent(itemsLayer, true);
            itemToDrop.transform.localScale = Vector3.one;

            Vector3 dropForce = CameraController.playerCameraInstance.transform.forward * 50f + CameraController.playerCameraInstance.transform.up * 40f;
            rb.AddForce(dropForce,ForceMode.Impulse);

            InventoryUI.instance.UpdateUI();
        }
    }

    public void SwapItems(int originIndex, int targetIndex)
    {
        if (originIndex == targetIndex) return;

        //Intercambio
        (items[targetIndex], items[originIndex]) = (items[originIndex], items[targetIndex]);
        InventoryUI.instance.UpdateUI();
    }
}