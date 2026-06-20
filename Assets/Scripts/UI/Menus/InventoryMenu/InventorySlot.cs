using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventorySlot : Slot, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{   
    private InputAction shift;

    void Start()
    {
        shift = InputSystem.actions.FindAction("Sprint");
    }

    public void OnPointerClick(PointerEventData eventData)
    {   
        if(!shift.IsPressed())
            return;

        int hotBarSize = InventoryController.instance.GetHotBarSize();
        int inventorySize = (int)InventoryController.instance.GetInventorySize();
        ItemStack item = InventoryController.instance.GetItem(slotIndex);
    
        switch (UIController.instance.GetCurrentState())
        {
            case UIState.Inventory or UIState.Crafting:
                if(slotIndex < hotBarSize)
                    InventoryController.instance.FastMove(slotIndex, hotBarSize, inventorySize);
                else
                    InventoryController.instance.FastMove(slotIndex, 0, hotBarSize);
                break;

            case UIState.Storage:
                StorageController storageController = UIController.instance.GetCurrentStorage().GetStorageController();
                  
                if(storageController.AddItem(item))
                    InventoryController.instance.RemoveItemByIndex(slotIndex);
                break;

            case UIState.Furnace:
                FurnaceController furnaceController = UIController.instance.GetCurrentFurnace().GetFurnaceController();

                if(furnaceController.AddInputFast(item) || furnaceController.AddFuel(item))
                    InventoryController.instance.RemoveItemByIndex(slotIndex);
    
                break;
        }      
    }

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

                if (item != null && InventoryController.instance.GetItem(slotIndex) == null)
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
                    if(InventoryController.instance.GetItem(slotIndex) != null)
                    {
                        storage.AddItem(originIndex, InventoryController.instance.GetItem(slotIndex));
                        InventoryController.instance.SetItem(slotIndex, item);
                    }
                    else
                    {
                        InventoryController.instance.AddItemFromStack(item, slotIndex);
                        storage.RemoveItem(originIndex);
                    }
                }
            }

            UpdateInventoryItemName();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {   
        if (transform.childCount == 0)
            return;

        UpdateInventoryItemName();    
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemName.text = "";
    }

    public void ClearItemName() => itemName.text = "";

    private void UpdateInventoryItemName()
    {
        ItemStack item = InventoryController.instance.GetItem(slotIndex);
        if(item != null)
            itemName.text = ItemDataBase.instance.GetByID(item.id).itemName;
        else
            itemName.text = "";
    }
}