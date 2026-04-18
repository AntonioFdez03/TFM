using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : Slot
{   
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        // Area para soltar el item -> 26% a cada lado 
        if(UIController.instance.GetCurrentState() == UIState.Inventory)
        {
            float margen = Screen.width * 0.26f;
            if (eventData.position.x < margen || eventData.position.x > Screen.width - margen)
            {   
                InventoryController.instance.DropItem(slotIndex);
                HotBarController.instance.UpdateHandItem();
            }
        }
    }

    public override void OnDrop(PointerEventData eventData)
    {
        Slot slotOrigen = eventData.pointerDrag?.GetComponent<Slot>();
        if (slotOrigen != null)
        {
            if(slotOrigen is InventorySlot)
            {
                InventoryController.instance.SwapItems(slotOrigen.GetSlotIndex(), slotIndex);
                HotBarController.instance.UpdateHandItem();
            }else if (slotOrigen is FurnaceSlot furnaceSlot)
            {
                int originIndex = furnaceSlot.GetSlotIndex();
                FurnaceController furnace = furnaceSlot.GetController();

                ItemStack item = furnace.GetItem(furnaceSlot.GetSlotType(),originIndex);

                if (item != null)
                {
                    InventoryController.instance.AddItemFromStack(item,slotIndex);
                    furnace.RemoveItem(furnaceSlot.GetSlotType(), originIndex);
                }
            }else if(slotOrigen is StorageSlot storageSlot)
            {
                int originIndex = storageSlot.GetSlotIndex();
                StorageController storage = storageSlot.GetController();
                ItemStack item = storage.GetItem(originIndex);

                if(item != null)
                {
                    InventoryController.instance.AddItemFromStack(item, slotIndex);
                    storage.RemoveItem(originIndex);
                }
            }
        }
    }
}