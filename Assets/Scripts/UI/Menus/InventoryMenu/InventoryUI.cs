using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] Transform gridPanel;
    [SerializeField] Transform hotBarPanel;
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Transform dragginLayer;
    [SerializeField] Transform itemHealthBar;

    private List<GameObject> slots = new();

    void Start()
    {
        Generate(hotBarPanel, InventoryController.instance.GetHotBarSize(), 0);
        Generate(gridPanel, InventoryController.instance.GetInventoryGridSize(),InventoryController.instance.GetHotBarSize());

        UpdateUI();
    }

    void Generate(Transform parent, int count, int start)
    {
        for (int i = start; i < start + count; i++)
        {
            GameObject slot = Instantiate(slotPrefab, parent);
            slot.name = "Slot_" + i;
            slot.GetComponent<Slot>().SetSlotIndex(i);
            slot.GetComponent<Slot>().SetDragginLayer(dragginLayer);
            slots.Add(slot);
        }
    }

    public void UpdateUI()
    {
        var items = InventoryController.instance.GetInventoryItems();

        for (int i = 0; i < items.Length && i < slots.Count; i++)
        {
            Image slot = slots[i].transform.GetChild(0).GetComponent<Image>();

            if (items[i] != null)
            {   
                ItemData data = ItemDataBase.instance.GetByID(items[i].id);

                if (data == null) continue;
                
                slot.sprite = data.icon;
                slot.gameObject.SetActive(true);
            }
            else
            {
                slot.sprite = null;
                slot.gameObject.SetActive(false);
            }

            UpdateEquipmentHealthBar(i);
        }
    }
    public void UpdateEquipmentHealthBar(int index)
    {
        ItemStack[] items = InventoryController.instance.GetInventoryItems();

        if (items[index] == null)
        {
            Transform existingBar = slots[index].transform.GetChild(0).Find("HealthBar");
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
            Transform healthBarInstance = slots[index].transform.GetChild(0).Find("HealthBar");

            if (healthBarInstance == null)
            {
                healthBarInstance = Instantiate(itemHealthBar, slots[index].transform.GetChild(0));
                healthBarInstance.name = "HealthBar";
                healthBarInstance.gameObject.SetActive(false);
            }

            if (currentHealth < maxHealth)
                healthBarInstance.gameObject.SetActive(true);

            Transform fill = healthBarInstance.Find("Fill");
            if (fill == null) return;

            if (!fill.TryGetComponent<Image>(out var fillImage)) return;

            fillImage.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        }
        else
        {
            Transform existingBar = slots[index].transform.Find("HealthBar");
            if (existingBar != null)
                Destroy(existingBar.gameObject);
        }
    }

    void OnEnable()
    {
        InventoryController.OnInventoryChanged += UpdateUI;
        UpdateUI();
    }

    void OnDisable()
    {
        InventoryController.OnInventoryChanged -= UpdateUI;
    }
}