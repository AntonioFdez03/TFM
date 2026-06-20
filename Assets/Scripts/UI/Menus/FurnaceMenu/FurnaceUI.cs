using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FurnaceUI : MonoBehaviour
{   
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform inputPanel;
    [SerializeField] private Transform fuelPanel;
    [SerializeField] private Transform outputPanel;
    [SerializeField] private GameObject furnaceBar;
    [SerializeField] Transform itemHealthBar;

    [SerializeField] private List<GameObject> fuelBarImages;
    [SerializeField] private Image arrow;
    
    private Transform dragginLayer;
    private FurnaceController furnaceController;
    private List<GameObject> inputSlots = new();
    private List<GameObject> outputSlots = new();
    private GameObject fuelSlot;

    public void SetFurnace(Furnace furnace)
    {
        furnaceController = furnace.GetComponent<FurnaceController>();
        furnaceController.OnFurnaceChanged += UpdateUI;
        GenerateSlots(inputPanel,3,FurnaceSlotType.Input);
        GenerateSlots(fuelPanel,1,FurnaceSlotType.Fuel);
        GenerateSlots(outputPanel,3,FurnaceSlotType.Output);
        UpdateUI();
    }

    public void SetDragginLayer(Transform layer) => dragginLayer = layer;

    private void GenerateSlots(Transform panel, int amount, FurnaceSlotType type)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, panel);
            slot.name = "Slot_" + i;
            slot.transform.localPosition = Vector3.zero;
            slot.GetComponent<Slot>().SetSlotIndex(i);
            slot.GetComponent<Slot>().SetDragginLayer(dragginLayer);
            slot.GetComponent<FurnaceSlot>().SetController(furnaceController);
            slot.GetComponent<FurnaceSlot>().SetFurnaceSlotType(type);

            if(panel == inputPanel)
                inputSlots.Add(slot);
            else if(panel == fuelPanel)
                fuelSlot = slot;
            else if(panel == outputPanel)
                outputSlots.Add(slot);

            if(type != FurnaceSlotType.Output)
                AddSliderBar(slot.transform);
        }
    }

    private void AddSliderBar(Transform slot)
    {
        GameObject sliderBar = Instantiate(furnaceBar, slot, false);

        RectTransform rt = sliderBar.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector3(0, -40, 0);
        rt.localScale = Vector3.one;

        sliderBar.SetActive(false);
        sliderBar.transform.SetParent(slot);
    }

    private void Update()
    {
        if(furnaceController == null) return;

        UpdateArrow();
    }
    private void UpdateUI()
    {
        if (furnaceController == null)
            return;

        UpdatePanel(inputPanel, furnaceController.GetInputItems());

        ItemStack[] fuelArray = new ItemStack[1];
        fuelArray[0] = furnaceController.GetFuelItem();
        UpdatePanel(fuelPanel, fuelArray);

        UpdatePanel(outputPanel, furnaceController.GetOutputItems());
        UpdateFuelBar();

        UpdateFuelHealthBar();
        UpdateInputsHealthBar(0);
        UpdateInputsHealthBar(1);
        UpdateInputsHealthBar(2);

        ClearSlotsNames();
    }

    void UpdatePanel(Transform panel, ItemStack[] items)
    {
        for (int i = 0; i < panel.childCount; i++)
        {
            Transform slotTransform = panel.GetChild(i);

            if (!slotTransform.TryGetComponent<FurnaceSlot>(out var slot))
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
        }
    }

    public void UpdateFuelHealthBar()
    {
        ItemStack item = furnaceController.GetItem(FurnaceSlotType.Fuel, 0);

        if (item == null)
        {
            Transform existingBar = fuelSlot.transform.GetChild(0).Find("HealthBar");
            if (existingBar != null)
                Destroy(existingBar.gameObject);

            return;
        }

        ItemData itemData = ItemDataBase.instance.GetByID(item.id);
        if (itemData == null)
            return;

        float currentHealth = item.currentHealth;

        if(itemData is DurableItemData durableItemData)
        {   
            float maxHealth = durableItemData.maxHealth;
            Transform healthBarInstance = fuelSlot.transform.GetChild(0).Find("HealthBar");

            if (healthBarInstance == null)
            {
                healthBarInstance = Instantiate(itemHealthBar, fuelSlot.transform.GetChild(0));
                healthBarInstance.name = "HealthBar";
                healthBarInstance.gameObject.SetActive(false);
            }

            if (currentHealth < maxHealth && currentHealth != 0)
                healthBarInstance.gameObject.SetActive(true);

            Transform fill = healthBarInstance.Find("Fill");
            if (fill == null) return;

            if (!fill.TryGetComponent<Image>(out var fillImage)) return;

            if(maxHealth != 0)
                fillImage.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
            else
                fillImage.fillAmount = 0;
        }
        else
        {
            Transform existingBar = fuelSlot.transform.Find("HealthBar");
            if (existingBar != null)
                Destroy(existingBar.gameObject);
        }
    }

    public void UpdateInputsHealthBar(int index)
    {
        ItemStack[] items = furnaceController.GetInputItems();

        if (items[index] == null)
        {
            Transform existingBar = inputSlots[index].transform.GetChild(0).Find("HealthBar");
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
            Transform healthBarInstance = inputSlots[index].transform.GetChild(0).Find("HealthBar");

            if (healthBarInstance == null)
            {
                healthBarInstance = Instantiate(itemHealthBar, inputSlots[index].transform.GetChild(0));
                healthBarInstance.name = "HealthBar";
                healthBarInstance.gameObject.SetActive(false);
            }

            if (currentHealth < maxHealth && currentHealth != 0)
                healthBarInstance.gameObject.SetActive(true);

            Transform fill = healthBarInstance.Find("Fill");
            if (fill == null) return;

            if (!fill.TryGetComponent<Image>(out var fillImage)) return;

            if(maxHealth != 0)
                fillImage.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
            else
                fillImage.fillAmount = 0;
        }
        else
        {
            Transform existingBar = inputSlots[index].transform.Find("HealthBar");
            if (existingBar != null)
                Destroy(existingBar.gameObject);
        }
    }



    private void UpdateFuelBar()
    {
        bool[] fuelStates = furnaceController.GetActiveFuelSlots();

        for(int i = 0; i < fuelBarImages.Count; i++)
        {
            fuelBarImages[i].SetActive(fuelStates[i]);
        }
    }

    private void UpdateArrow()
    {
        arrow.fillAmount = furnaceController.GetCurrentTimer() / furnaceController.GetCurrentBakeDuration();
    }

    private void ClearSlotsNames()
    {   
        fuelSlot.GetComponent<FurnaceSlot>().ClearItemName();
        for(int i = 0; i < 3; i++)
        {   
            inputSlots[i].GetComponent<FurnaceSlot>().ClearItemName();
            outputSlots[i].GetComponent<FurnaceSlot>().ClearItemName();
        }
    }

    private void OnDisable()
    {   
        if (furnaceController != null)
            furnaceController.OnFurnaceChanged -= UpdateUI;
    }
}