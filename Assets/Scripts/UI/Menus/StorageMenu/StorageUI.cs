using UnityEngine;
using UnityEngine.UI;

public class StorageUI : MonoBehaviour
{   
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform gridPanel;
    [SerializeField] private Transform itemHealthBar;
    private Transform dragginLayer;
    private StorageController storageController;

    public void SetStorage(Storage storage)
    {
        storageController = storage.GetComponent<StorageController>();
        storageController.OnInventoryChanged += UpdateUI;
        GenerateSlots(gridPanel,storageController.GetStorageSize());
        UpdateUI();
    }

    public void SetDragginLayer(Transform layer) => dragginLayer = layer;

    private void GenerateSlots(Transform panel, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, panel);
            slot.name = "Slot_" + i;
            slot.transform.localPosition = Vector3.zero;
            slot.GetComponent<Slot>().SetSlotIndex(i);
            slot.GetComponent<Slot>().SetDragginLayer(dragginLayer);
            slot.GetComponent<StorageSlot>().SetController(storageController);
        }
    }

    private void UpdateUI()
    {
        if (storageController == null)
            return;

        ItemStack[] items = storageController.GetItems();
        for (int i = 0; i < gridPanel.childCount; i++)
        {
            Transform slotTransform = gridPanel.GetChild(i);

            if (!slotTransform.TryGetComponent<StorageSlot>(out var slot))
                continue;

            int index = slot.GetSlotIndex();

            if (slotTransform.childCount == 0)
                continue;

            GameObject iconGO = slotTransform.GetChild(0).gameObject;
            Image iconImage = iconGO.GetComponent<Image>();

            if (index < items.Length && items[index] != null)
            {
                ItemStack item = items[index];
                ItemData data = ItemDataBase.instance.GetByID(item.id);

                if (data != null)
                {
                    iconImage.sprite = data.icon;
                    iconGO.SetActive(true);
                }
                else
                {
                    iconGO.SetActive(false);
                }
            }
            else
            {
                iconImage.sprite = null;
                iconGO.SetActive(false);
            }
        
            UpdateEquipmentHealthBar(i);
        }
    }

    public void UpdateEquipmentHealthBar(int index)
    {
        ItemStack[] items = storageController.GetItems();

        if (items[index] == null)
        {
            Transform existingBar = gridPanel.GetChild(index).Find("HealthBar");
            if (existingBar != null)
                Destroy(existingBar.gameObject);

            return;
        }

        ItemData item = ItemDataBase.instance.GetByID(items[index].id);
        print("Item data: " + item);
        if (item == null)
            return;

        float currentHealth = items[index].currentHealth;

        if(item is DurableItemData durableItemData)
        {   
            print("Entra");
            float maxHealth = durableItemData.maxHealth;
            Transform healthBarInstance = gridPanel.GetChild(index).GetChild(0).Find("HealthBar");

            if (healthBarInstance == null)
            {
                healthBarInstance = Instantiate(itemHealthBar, gridPanel.GetChild(index).GetChild(0));
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
            Transform existingBar = gridPanel.GetChild(index).Find("HealthBar");
            if (existingBar != null)
                Destroy(existingBar.gameObject);
        }
    }

    private void OnDisable()
    {
        if (storageController != null)
            storageController.OnInventoryChanged -= UpdateUI;
    }
}