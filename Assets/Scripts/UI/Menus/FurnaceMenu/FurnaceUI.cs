using UnityEngine;
using UnityEngine.UI;

public class FurnaceUI : MonoBehaviour
{   
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform inputPanel;
    [SerializeField] private Transform fuelPanel;
    [SerializeField] private Transform outputPanel;
    private Transform dragginLayer;
    private FurnaceController furnaceController;

    public void SetFurnace(Furnace furnace)
    {
        furnaceController = furnace.GetComponent<FurnaceController>();
        furnaceController.OnInventoryChanged += UpdateUI;
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
        }
    }

    private void UpdateUI()
    {
        if (furnaceController == null)
            return;

        UpdatePanel(inputPanel, furnaceController.GetInputObjects());

        ItemStack[] fuelArray = new ItemStack[1];
        fuelArray[0] = furnaceController.GetFuelObject();
        UpdatePanel(fuelPanel, fuelArray);

        UpdatePanel(outputPanel, furnaceController.GetOutputObjects());
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

    private void OnDisable()
    {
        if (furnaceController != null)
            furnaceController.OnInventoryChanged -= UpdateUI;
    }
}