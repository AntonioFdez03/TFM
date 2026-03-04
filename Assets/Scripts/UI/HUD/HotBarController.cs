using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
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
    [SerializeField] GameObject slotPrefab;
    private List<Image> inventorySlots = new();
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
        GenerateSlots(hotBarPanel, InventoryController.instance.GetHotBarSize(), 0);
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

        InventoryController.OnInventoryChanged += UpdateHotBarUI;
        RefreshHandItem();
        DropCurrentItem();
    }

     public void GenerateSlots(Transform parent, int count, int startIndex)
    {
        for (int i = startIndex; i < count + startIndex; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, parent);
            newSlot.name = "Slot_" + i;

            RectTransform rect = newSlot.GetComponent<RectTransform>();
            if (rect != null) rect.sizeDelta = new Vector2(110f,110f);
        

            InventorySlot scriptSlot = newSlot.GetComponent<InventorySlot>();
            if (scriptSlot != null)
            {
                scriptSlot.slotIndex = i;
            }

            inventorySlots.Add(newSlot.GetComponent<Image>());
        }
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

        ItemData data = slots[selectedIndex].GetComponentInChildren<ItemData>();
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
            currentItem = Instantiate(newPrefab);
            currentItem.transform.SetParent(handSlot, false);
            currentItem.transform.localScale = Vector3.one;
            currentItem.SetActive(true);
            currentItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            DisablePhysics();

            currentItemBehaviour = currentItem.GetComponent<ItemBehaviour>();
        }

    }

    private void DisablePhysics()
    {
        Rigidbody rb = currentItem.GetComponent<Rigidbody>();
        Collider bc = currentItem.GetComponent<Collider>();
        if (rb != null && bc != null)
        {
            print("Entra");
            rb.isKinematic = true;
            rb.detectCollisions = false;
            bc.enabled = false;
        }
    }

    private void DropCurrentItem()
    {
        if (dropItem.WasPressedThisFrame() && currentItem != null)
        {
            Rigidbody rb = currentItem.GetComponent<Rigidbody>();
            ItemData data = slots[selectedIndex].GetComponentInChildren<ItemData>();
            print(rb);
            print(data);
            if(rb != null && data != null)
            {
                InventoryController.instance.DropItem(selectedIndex);
                data.Clear();
                Destroy(currentItem);
            }
            
        }
    }

    public void UpdateHotBarUI()
    {
        GameObject[] items = InventoryController.instance.GetInventoryItems();

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                continue;

            // Si el slot tiene hijo (Icon)
            if (slots[i].childCount > 0)
            {
                GameObject slotItem = slots[i].GetChild(0).gameObject;

                if (i < items.Length && items[i] != null)
                {
                    ItemData originalData = items[i].GetComponent<ItemData>();
                    ItemData uiData = slotItem.GetComponent<ItemData>();

                    if (originalData != null && uiData != null)
                    {
                        uiData.CopyFrom(originalData);
                        slotItem.GetComponent<Image>().sprite = originalData.GetItemIcon();
                        slotItem.SetActive(true);
                    }
                }
                else
                {
                    ItemData uiData = slotItem.GetComponent<ItemData>();
                    if (uiData != null)
                        uiData.SetItemPrefab(null);

                    slotItem.SetActive(false);
                }
            }
        }

        // Opcional: actualizar mano también
        RefreshHandItem();
    }
}