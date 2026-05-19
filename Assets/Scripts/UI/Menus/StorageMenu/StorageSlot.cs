using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StorageSlot : Slot
{   
    private StorageController storageController;
    public void SetController(StorageController controller) => storageController = controller;
    public StorageController GetController() => storageController;

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
        
        Slot originSlot = eventData.pointerDrag?.GetComponentInParent<Slot>();
        int originIndex = originSlot.GetSlotIndex();
        int index = transform.GetSiblingIndex();

        if (storageController == null)
            return;

        ItemStack item = null;
        if(originSlot is InventorySlot)
        {
            item = InventoryController.instance.GetInventoryItems()[originIndex];
            if(item != null)
            {
                ItemStack storageItem = storageController.GetItem(index);

                // Swap
                if(storageItem != null)
                {
                    storageController.AddItem(index, item);
                    InventoryController.instance.SetItem(originIndex, storageItem);
                }
                // Mover normal
                else
                {
                    storageController.AddItem(index, item);
                    InventoryController.instance.RemoveItem(item);
                }
            }
        }else if(originSlot is StorageSlot)
        {
            storageController.SwapItems(originIndex,slotIndex);
        }

    }
}