using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    private ItemStack currentItem;
    private ItemStack lastHandItem;
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
        UpdateHotBarUI();
    }

    public ItemStack GetCurrentItem() => currentItem;
    public ItemBehaviour GetCurrentItemBehaviour() => currentItemBehaviour;
    public int GetSelectedIndex() => selectedIndex;
    public GameObject GetHandItem() => handItemInstance;
    public void SetHandItem(GameObject item) => handItemInstance = item; 

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

        float scroll = Mouse.current.scroll.ReadValue().y;
        int currentIndex = 0;

        if (scroll < 0)
        {
            currentIndex = (selectedIndex + 1) % slots.Length;
            MoveSelectorFrame(currentIndex);
        }
        else if (scroll > 0)
        {
            currentIndex = (selectedIndex - 1 + slots.Length) % slots.Length;
            MoveSelectorFrame(currentIndex);
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
                scriptSlot.SetSlotIndex(i);
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

    public void MoveSelectorFrame(int index)
    {   
        if (index >= 0 && index < slots.Length && slots[index] != null && index != selectedIndex)
        {
            selectorFrame.SetParent(slots[index]);
            selectorFrame.anchoredPosition = Vector2.zero;
            selectorFrame.localScale = Vector3.one;
            selectedIndex = index;
            if(ArmController.instance.IsMoving())
                ArmController.instance.ResetArm();
            
            UpdateHandItem();
        }
    }

    public void UpdateHandItem()
    {
        ItemStack[] items = InventoryController.instance.GetInventoryItems();

        if (selectedIndex < 0 || selectedIndex >= items.Length)
            return;

        currentItem = items[selectedIndex];

        if (lastHandItem == currentItem)
            return;

        lastHandItem = currentItem;
    
        // limpiar anterior
        if (handItemInstance != null)
            Destroy(handItemInstance);

        if (lastPlaceableItem != null)
        {
            lastPlaceableItem.DeleteSilhouette();
            lastPlaceableItem = null;
        }

        if (currentItem == null)
        {
            handItemInstance = null;
            currentItemBehaviour = null;
            currentPrefab = null;
            return;
        }

        ItemData item = ItemDataBase.instance.GetByID(currentItem.id);

        currentPrefab = item.prefab;
        handItemInstance = Instantiate(currentPrefab);
        currentItemBehaviour = handItemInstance.GetComponent<ItemBehaviour>();

        if (currentItemBehaviour != null)
        {
            currentItemBehaviour.Initialize(currentItem);
            currentItemBehaviour.SetCurrentHealth(currentItem.currentHealth);
        }

        if (currentItemBehaviour.GetData().showInHand)
        {
            handItemInstance.transform.SetParent(handSlot, false);
            handItemInstance.transform.localScale = Vector3.one;
            handItemInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            DisablePhysics();
        }
        else
        {
            lastPlaceableItem = currentItemBehaviour as PlaceableBehaviour;
            handItemInstance.SetActive(false);
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
            if(!InventoryController.instance.DropItem(selectedIndex))
                return;

            //ArmController.instance.DropAnimation();

            if (handItemInstance != null)
                Destroy(handItemInstance);
        }
    }

    public void UpdateHotBarUI()
    {
        ItemStack[] items = InventoryController.instance.GetInventoryItems();

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                continue;

            if (slots[i].childCount > 0)
            {
                GameObject slotItem = slots[i].GetChild(0).gameObject;

                if (i < items.Length && items[i] != null)
                {
                    ItemData item = ItemDataBase.instance.GetByID(items[i].id);

                    if (item != null)
                    {
                        slotItem.GetComponent<Image>().sprite = item.icon;
                        slotItem.SetActive(true);
                    }
                }
                else
                {
                    slotItem.SetActive(false);
                }
            }

            UpdateEquipmentHealthBar(i);
        }
        UpdateHandItem();
    }

    public void UpdateEquipmentHealthBar(int index)
    {
        ItemStack[] items = InventoryController.instance.GetInventoryItems();

        if (items[index] == null)
        {
            Transform existingBar = slots[index].GetChild(0).Find("HealthBar");
            if (existingBar != null)
                Destroy(existingBar.gameObject);

            return;
        }

        ItemData item = ItemDataBase.instance.GetByID(items[index].id);
        if (item == null)
            return;

        float currentHealth = items[index].currentHealth;

        if(item is DurableItemData durableItemData)
        {   
            float maxHealth = durableItemData.maxHealth;
            Transform healthBarInstance = slots[index].GetChild(0).Find("HealthBar");

            if (healthBarInstance == null)
            {
                healthBarInstance = Instantiate(itemHealthBar, slots[index].GetChild(0));
                healthBarInstance.name = "HealthBar";
                healthBarInstance.gameObject.SetActive(false);
            }

            // Mostrar si no está al 100%
            if (currentHealth < maxHealth)
                healthBarInstance.gameObject.SetActive(true);

            Transform fill = healthBarInstance.Find("Fill");
            if (fill == null) return;

            if (!fill.TryGetComponent<Image>(out var fillImage)) return;

            fillImage.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        }
        else
        {
            Transform existingBar = slots[index].Find("HealthBar");
            if (existingBar != null)
                Destroy(existingBar.gameObject);
        }
    }
}