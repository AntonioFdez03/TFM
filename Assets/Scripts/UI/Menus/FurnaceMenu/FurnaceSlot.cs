using UnityEngine;
using UnityEngine.EventSystems;

public enum FurnaceSlotType {None, Input, Fuel, Output}
public class FurnaceSlot : Slot
{   
    [SerializeField] private FurnaceSlotType slotType;

    private FurnaceController furnaceController;

    public void SetController(FurnaceController controller)
    {
        furnaceController = controller;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        ItemData originalItemData = eventData.pointerDrag?.GetComponentInChildren<ItemData>();
        int index = transform.GetSiblingIndex();

        if (originalItemData != null)
        {
            switch (slotType)
            {
                case FurnaceSlotType.Input:
                    furnaceController.AddInput(index,originalItemData);
                    //InventoryController.instance.RemoveItem(originalItemData);
                    break;

                case FurnaceSlotType.Fuel:
                break;

                case FurnaceSlotType.Output:
                break;
            }
        }
    }
}