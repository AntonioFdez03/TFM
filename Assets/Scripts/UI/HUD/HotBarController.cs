using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class HotBarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] InventoryController inventoryController;
    [SerializeField] Transform hotBarPanel;
    [SerializeField] RectTransform selectorFrame;
    [SerializeField] Transform handSlot;

    [Header("Settings")]
    [SerializeField] [Range(0, 6)] private int selectedIndex = 0;
    
    private Transform[] slots = new Transform[7];
    private GameObject currentItem;
    private InputAction dropItem; 

    void Start()
    {   
        dropItem = InputSystem.actions.FindAction("Drop");
        LoadSlots();
        MoveSelectorFrame(selectedIndex);
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Cambio de slot mediante teclado numérico
        for (int i = 0; i < slots.Length; i++)
        {
            Key targetKey = (Key)((int)Key.Digit1 + i);
            if (Keyboard.current[targetKey].wasPressedThisFrame)
            {
                MoveSelectorFrame(i);
            }
        }

        RefreshHandItem();
        DropCurrentItem();
    }

    private void LoadSlots()
    {
        for (int i = 0; i < 7; i++)
        {
            Transform slotFound = hotBarPanel.Find("Slot_" + i);
            if (slotFound != null) slots[i] = slotFound;
        }
    }

    private void MoveSelectorFrame(int index)
    {
        if (index >= 0 && index < slots.Length && slots[index] != null)
        {
            selectorFrame.SetParent(slots[index]);
            selectorFrame.anchoredPosition = Vector2.zero;
            selectorFrame.localScale = Vector3.one;
            selectedIndex = index;
        }
    }

    public void RefreshHandItem()
    {   
        ItemData data = slots[selectedIndex].GetComponentInChildren<ItemData>(true);
        print(data);
        //Si no hay objeto en la mano, y tenemos datos de prefab del slot actual
        if (currentItem == null && data.itemPrefab != null)
        {  
            currentItem = Instantiate(data.itemPrefab, handSlot);
            currentItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            currentItem.transform.localScale = Vector3.one;
            DisablePhysics();
        }
        else if(data.itemPrefab == null)
            Destroy(currentItem);
    }

    private void DisablePhysics()
    {
        Rigidbody rb = currentItem.GetComponent<Rigidbody>();
        BoxCollider bc = currentItem.GetComponent<BoxCollider>();
        if (rb != null && bc != null)
        {
            rb.isKinematic = true;
            bc.enabled = false;
        }
    }

    private void DropCurrentItem()
    {
        if (dropItem.WasPressedThisFrame() && currentItem != null)
        {
            print("Drop");
            Rigidbody rb = currentItem.GetComponent<Rigidbody>();
            ItemData itemData = slots[selectedIndex].GetComponentInChildren<ItemData>(true);
            print(rb);
            print(itemData);
            if(rb != null && itemData != null)
            {
                print("Se ha dropeado");
                inventoryController.DropItem(selectedIndex);
                itemData.itemName = null;
                itemData.itemIcon = null;
                itemData.itemPrefab = null;
                Destroy(currentItem);
            }
            
        }
    }
}