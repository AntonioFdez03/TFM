using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum FurnaceSlotType {None, Input, Fuel, Output}
public class FurnaceSlot : Slot, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{   
    [SerializeField] private FurnaceSlotType furnaceSlotType;

    private FurnaceController furnaceController;

    private InputAction shift;

    void Start()
    {
        shift = InputSystem.actions.FindAction("Sprint");
    }
    
    public FurnaceController GetController() => furnaceController;
    public FurnaceSlotType GetSlotType() => furnaceSlotType;
    public void SetController(FurnaceController controller) => furnaceController = controller;
    public void SetFurnaceSlotType(FurnaceSlotType type) => furnaceSlotType = type;

    public void OnPointerClick(PointerEventData eventData)
    {   
        if(!shift.IsPressed())
            return;

        ItemStack item = furnaceController.GetItem(furnaceSlotType, slotIndex);
    
        if(InventoryController.instance.AddItemFromStack(item))
            furnaceController.RemoveItem(furnaceSlotType, slotIndex);
          
    }
    public override void OnDrop(PointerEventData eventData)
    {   
        
        Slot originSlot = eventData.pointerDrag?.GetComponentInParent<Slot>();
        int originIndex = originSlot.GetSlotIndex();
        int index = transform.GetSiblingIndex();

        if (furnaceController == null)
            return;

        ItemStack item;
        if(originSlot is InventorySlot)
        {
            item = InventoryController.instance.GetInventoryItems()[originIndex];
            if(item != null)
            {
                switch (furnaceSlotType)
                {
                    case FurnaceSlotType.Input:
                        if(furnaceController.GetItem(FurnaceSlotType.Input,index) == null)
                        {
                            if(furnaceController.AddInput(index,item))
                                InventoryController.instance.RemoveItem(item);
                        }
                        break;

                    case FurnaceSlotType.Fuel:
                        if(furnaceController.GetItem(FurnaceSlotType.Fuel,index) == null)
                        {
                            if(furnaceController.AddFuel(item))
                                InventoryController.instance.RemoveItem(item);
                        }
                    break;
                }
            }
        }else if(originSlot is FurnaceSlot furnaceSlot)
        {
            item = furnaceController.GetItem(furnaceSlot.GetSlotType(),furnaceSlot.GetSlotIndex());
            switch (furnaceSlotType)
                {
                    case FurnaceSlotType.Input:
                        if(furnaceController.GetItem(FurnaceSlotType.Input,index) == null)
                        {
                            if(furnaceController.AddInput(index,item))
                                furnaceController.RemoveItem(furnaceSlot.GetSlotType(),furnaceSlot.GetSlotIndex());
                        }
                        break;

                    case FurnaceSlotType.Fuel:
                        if(furnaceController.GetItem(FurnaceSlotType.Fuel,index) == null)
                        {
                            if(furnaceController.AddFuel(item))
                                furnaceController.RemoveItem(furnaceSlot.GetSlotType(),furnaceSlot.GetSlotIndex());
                        }
                        break;
                }
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {   
        if (transform.childCount == 0)
            return;

        UpdateFurnaceItemName();    
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemName.text = "";
    }

    public void ClearItemName() => itemName.text = "";

    private void UpdateFurnaceItemName()
    {
        ItemStack item = furnaceController.GetItem(furnaceSlotType, slotIndex);
        if(item != null)
            itemName.text = ItemDataBase.instance.GetByID(item.id).itemName;
        else
            itemName.text = "";
    }
}