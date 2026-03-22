using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;

public class HotBarController : MonoBehaviour
{   
    public static HotBarController instance;

    [Header("References")]
    [SerializeField] Transform hotBarPanel;
    [SerializeField] RectTransform selectorFrame;
    [SerializeField] Transform handSlot;
    [SerializeField] Transform itemHealthBar;

    [Header("Settings")]
    [Range(0, 6)] private int selectedIndex = 0;
    
    private Transform[] slots;
    private GameObject currentItem;
    private GameObject handItemInstance;
    private ItemBehaviour currentItemBehaviour;
    private GameObject currentPrefab;
    private PlaceableBehaviour lastPlaceableItem;
    private InputAction dropItem; 
    [SerializeField] GameObject slotPrefab;
    private List<Image> inventorySlots = new();

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        slots = new Transform[InventoryController.instance.GetHotBarSize()];
    }
    void Start()
    {   
        dropItem = InputSystem.actions.FindAction("Drop");
        GenerateSlots(hotBarPanel, InventoryController.instance.GetHotBarSize(), 0);
        LoadSlots();
        MoveSelectorFrame(selectedIndex);
    }

    public GameObject GetCurrentItem() => currentItem;
    public ItemBehaviour GetCurrentItemBehaviour() => currentItemBehaviour;

    void Update()
    {   
        if (Keyboard.current == null) return;

        for (int i = 0; i < slots.Length; i++)
        {
            Key targetKey = (Key)((int)Key.Digit1 + i);
            if (Keyboard.current[targetKey].wasPressedThisFrame)
            {
                MoveSelectorFrame(i);
            }
        }
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
        if (index >= 0 && index < slots.Length && slots[index] != null && index != selectedIndex)
        {
            selectorFrame.SetParent(slots[index]);
            selectorFrame.anchoredPosition = Vector2.zero;
            selectorFrame.localScale = Vector3.one;
            selectedIndex = index;
            RefreshHandItem();
        }
    }

    public void RefreshHandItem()
    {
        GameObject[] items = InventoryController.instance.GetInventoryItems();

        if (selectedIndex < 0 || selectedIndex >= items.Length)
            return;

        // objeto real del inventario
        currentItem = items[selectedIndex];

        if (lastPlaceableItem != null)
        {
            lastPlaceableItem.DeleteSilhouette();
            lastPlaceableItem = null;
        }
        // destruir instancia visual anterior
        if (handItemInstance != null)
            Destroy(handItemInstance);

        // slot vacío
        if (currentItem == null)
        {
            handItemInstance = null;
            currentItemBehaviour = null;
            currentPrefab = null;
            return;
        }

        ItemData data = currentItem.GetComponent<ItemData>();
        currentPrefab = data.GetItemPrefab();

        // crear instancia visual
        handItemInstance = Instantiate(currentPrefab);
        currentItemBehaviour = handItemInstance.GetComponent<ItemBehaviour>();

        print("LastPlaceable: " + lastPlaceableItem);
        if (currentItemBehaviour is not PlaceableBehaviour)
        {   
            handItemInstance.transform.SetParent(handSlot, false);
            handItemInstance.transform.localScale = Vector3.one;
            handItemInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            handItemInstance.SetActive(true);

            DisablePhysics();
        }
        else
        {   
            print("last placeable asignado");
            lastPlaceableItem = currentItemBehaviour as PlaceableBehaviour;
        }
    }

    private void DisablePhysics()
    {
        if (handItemInstance == null) return;

        Rigidbody rb = handItemInstance.GetComponent<Rigidbody>();
        Collider bc = handItemInstance.GetComponent<Collider>();

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
            InventoryController.instance.DropItem(selectedIndex);

            if (handItemInstance != null)
                Destroy(handItemInstance);
        }
    }

    public void UpdateHotBarUI()
    {
        GameObject[] items = InventoryController.instance.GetInventoryItems();

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                continue;

            if (slots[i].childCount > 0)
            {
                GameObject slotItem = slots[i].GetChild(0).gameObject;

                if (i < InventoryController.instance.GetHotBarSize() && items[i] != null)
                {
                    ItemData originalData = items[i].GetComponent<ItemData>();
                    ItemData uiData = slotItem.GetComponent<ItemData>();

                    if (originalData != null && uiData != null)
                    {
                        uiData.CopyFrom(originalData);
                        slotItem.GetComponent<Image>().sprite = originalData.GetItemIcon();
                        slotItem.SetActive(true);

                        if(uiData.GetItemType() == ItemType.Tool)
                        {
                            print("Se muestra la barra");
                            Instantiate(itemHealthBar, slots[i]);
                        }
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
        RefreshHandItem();
    }
}