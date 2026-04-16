using UnityEngine;
using UnityEngine.UI;

public class FurnaceUI : MonoBehaviour
{   
    [SerializeField] private Transform inputPanel;
    [SerializeField] private Transform fuelPanel;
    [SerializeField] private Transform outputPanel;
    private FurnaceController furnaceController;

    public void SetFurnace(Furnace furnace)
    {
        furnaceController = furnace.GetComponent<FurnaceController>();
        furnaceController.OnInventoryChanged += UpdateUI;
        LoadFurnaceData();
    }

    void LoadFurnaceData()
    {
        LoadController(inputPanel);
        LoadController(fuelPanel);
        LoadController(outputPanel);
        UpdateUI();
    }

    private void LoadController(Transform panel)
    {
        foreach (Transform child in panel)
        {
            FurnaceSlot slot = child.GetComponent<FurnaceSlot>();
            if (slot != null)
            {
                slot.SetController(furnaceController);
            }
        }
    }

    private void UpdateUI()
    {
        if (furnaceController == null)
            return;

        ItemData[] inputItems = furnaceController.GetInputObjects();

        for (int i = 0; i < inputPanel.childCount; i++)
        {
            Transform slotTransform = inputPanel.GetChild(i);
            FurnaceSlot slot = slotTransform.GetComponent<FurnaceSlot>();

            if (slot == null) continue;

            int index = slot.GetSlotIndex();

            // Icono UI (primer hijo del slot)
            if (slotTransform.childCount == 0) continue;

            GameObject iconGO = slotTransform.GetChild(0).gameObject;
            Image iconImage = iconGO.GetComponent<Image>();

            if (index < inputItems.Length && inputItems[index] != null)
            {
                ItemData item = inputItems[index];

                // Asignar sprite
                iconImage.sprite = item.GetItemIcon();
                iconGO.SetActive(true);

                // (Opcional) copiar datos al icono si usas ItemData en UI
                ItemData uiData = iconGO.GetComponent<ItemData>();
                if (uiData != null)
                {
                    uiData.CopyFrom(item);
                }
            }
            else
            {
                // Vaciar slot
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