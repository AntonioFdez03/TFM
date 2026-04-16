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
                HotBarController.instance.RefreshHandItem();
            }
        }
    }

    public override void OnDrop(PointerEventData eventData)
    {
        Slot slotOrigen = eventData.pointerDrag?.GetComponent<Slot>();

        if (slotOrigen != null)
        {
            if(slotOrigen as InventorySlot)
            {
                InventoryController.instance.SwapItems(slotOrigen.GetSlotIndex(), slotIndex);
                HotBarController.instance.RefreshHandItem();
            }
        }
    }
}