using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class StorageSlot : Slot, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{   
    private StorageController storageController;
    public void SetController(StorageController controller) => storageController = controller;
    public StorageController GetController() => storageController;
    private InputAction shift;

    void Start()
    {
        shift = InputSystem.actions.FindAction("Sprint");
    }

    public void OnPointerClick(PointerEventData eventData)
    {   
        if (shift.IsPressed())
        {   
            ItemStack item = storageController.GetItem(slotIndex);
            InventoryController inventory = InventoryController.instance;

            if(inventory.AddItemFromStack(item))
                storageController.RemoveItem(slotIndex);
            }
    }     

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

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

        UpdateStorageItemName();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {   
        if (transform.childCount == 0)
            return;

        UpdateStorageItemName();    
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemName.text = "";
    }

    private void UpdateStorageItemName()
    {
        ItemStack item = storageController.GetItem(slotIndex);
        if(item != null)
            itemName.text = ItemDataBase.instance.GetByID(item.id).itemName;
        else
            itemName.text = "";
    }
}