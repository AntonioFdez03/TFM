using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class HotBarController : MonoBehaviour
{   
    public static HotBarController hotBarInstance;

    [Header("References")]
    [SerializeField] Transform hotBarPanel;
    [SerializeField] RectTransform selectorFrame;
    [SerializeField] Transform handSlot;

    [Header("Settings")]
    [Range(0, 6)] private int selectedIndex = 0;
    
    private Transform[] slots = new Transform[7];
    private GameObject currentItem;
    private ItemBehaviour currentItemBehaviour;
    private GameObject currentPrefab;
    private InputAction dropItem; 

    public GameObject GetCurrentItem() => currentItem;
    public ItemBehaviour GetCurrentItemBehaviour() => currentItemBehaviour;

    void Awake()
    {
        if(hotBarInstance != null && hotBarInstance != this)
        {
            Destroy(gameObject);
            return;
        }
        hotBarInstance = this;
    }
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
                currentPrefab = null;
                currentItemBehaviour = null;
            }
            return;
        }

        // Caso 2: hay item distinto → reemplazar
        if (currentItem == null || currentPrefab != newPrefab)
        {
            if (currentItem != null)
            {
                print("Se destruye el item");
                Destroy(currentItem);
            }

            currentPrefab = newPrefab;
            currentItem = Instantiate(newPrefab, handSlot);
            currentItem.SetActive(true);
            currentItem.transform.localPosition = Vector3.zero;
            currentItem.transform.localRotation = Quaternion.identity;
            DisablePhysics();

            currentItemBehaviour = currentItem.GetComponentInChildren<ItemBehaviour>();
        }

    }

    private void DisablePhysics()
    {
        Rigidbody rb = currentItem.GetComponentInChildren<Rigidbody>();
        Collider bc = currentItem.GetComponentInChildren<Collider>();
        if (rb != null && bc != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
            bc.enabled = false;
        }
    }

    private void DropCurrentItem()
    {
        if (dropItem.WasPressedThisFrame() && currentItem != null)
        {
            print("Drop");
            Rigidbody rb = currentItem.GetComponentInChildren<Rigidbody>(true);
            ItemData data = slots[selectedIndex].GetComponentInChildren<ItemData>(true);
            print(rb);
            print(data);
            if(rb != null && data != null)
            {
                print("Se ha dropeado");
                InventoryController.inventoryInstance.DropItem(selectedIndex);
                data.Clear();
                Destroy(currentItem);
            }
            
        }
    }
}