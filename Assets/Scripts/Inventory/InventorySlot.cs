using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{   
    [Header("References")]
    private Transform draggingLayer; // Capa en la que mover el icono mientras se arrastra para mostrar por encima del resto

    public int slotIndex;

    private Image originalIcon;
    private GameObject cloneIcon;

    public void SetDragginLayer(Transform layer) => draggingLayer = layer;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (transform.childCount == 0) return; //Si no hay nada en el slot

        originalIcon = transform.GetChild(0).GetComponent<Image>();

        if (originalIcon == null || !originalIcon.gameObject.activeSelf) return;

        // Ocultar el icono original
        originalIcon.color = new Color(1, 1, 1, 0f); 

        cloneIcon = new GameObject("cloneIcon");
        cloneIcon.transform.SetParent(draggingLayer);
        
        Image cloneImage = cloneIcon.AddComponent<Image>();
        cloneImage.sprite = originalIcon.sprite;
        cloneImage.raycastTarget = false; 

        RectTransform rt = cloneIcon.GetComponent<RectTransform>();
        rt.localScale = Vector3.one; 
        rt.sizeDelta = new Vector2(150f, 150f); // Aumentamos un poco el tamaño
        rt.pivot = new Vector2(0.5f, 0.5f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (cloneIcon != null)
        {
            // Seguir la posicion del cursor
            cloneIcon.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (cloneIcon != null) {
            originalIcon.color = Color.white;
            Destroy(cloneIcon);
        }

        // Area para soltar el item -> 26% a cada lado 
        float margen = Screen.width * 0.26f;
        if (eventData.position.x < margen || eventData.position.x > Screen.width - margen)
        {   
            InventoryController.inventoryInstance.DropItem(slotIndex);
            HotBarController.hotBarInstance.RefreshHandItem();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot slotOrigen = eventData.pointerDrag?.GetComponent<InventorySlot>();

        //Intercambia el slot destino con el actual, aunque esté vacío
        if (slotOrigen != null)
        {
            InventoryController.inventoryInstance.SwapItems(slotOrigen.slotIndex, this.slotIndex);
            HotBarController.hotBarInstance.RefreshHandItem();
        }
    }
}