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
    [Range(0, 6)] private int selectedIndex = 0;
    
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
        if (slots[selectedIndex] == null) return;

        ItemData data = slots[selectedIndex].GetComponentInChildren<ItemData>(true);
        GameObject newPrefab = data != null ? data.GetItemPrefab() : null;

        // Caso 1: slot vacío → quitar item
        if (newPrefab == null)
        {
            if (currentItem != null)
            {
                Destroy(currentItem);
                currentItem = null;
            }
            return;
        }

        // Caso 2: hay item distinto → reemplazar
        if (currentItem == null || currentItem.name != newPrefab.name)
        {
            if (currentItem != null)
                Destroy(currentItem);

            currentItem = Instantiate(newPrefab, handSlot);
            currentItem.SetActive(true);
            currentItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            currentItem.transform.localScale = Vector3.one;
            DisablePhysics();
        }
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
            ItemData data = slots[selectedIndex].GetComponentInChildren<ItemData>(true);
            print(rb);
            print(data);
            if(rb != null && data != null)
            {
                print("Se ha dropeado");
                inventoryController.DropItem(selectedIndex);
                data.SetItemName(null);
                data.SetItemIcon(null);
                data.SetItemPrefab(null);
                Destroy(currentItem);
            }
            
        }
    }

    //Método para saber el behaviour del item actual
    public ItemBehaviour GetCurrentItemBehaviour()
    {
        if (currentItem == null) return null;
        return currentItem.GetComponent<ItemBehaviour>();
    }
}