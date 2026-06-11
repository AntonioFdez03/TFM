using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{   
    [SerializeField] protected int slotIndex;
    [SerializeField] protected Transform draggingLayer;

    protected Image originalIcon;
    protected GameObject cloneIcon;

    public void SetSlotIndex(int i) => slotIndex = i;
    public int GetSlotIndex() => slotIndex;
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

        Transform originalBar = originalIcon.transform.Find("HealthBar");

        if (originalBar != null)
        {   
            originalBar.gameObject.SetActive(false);
            Transform cloneBar =
                Instantiate(originalBar, cloneIcon.transform);

            cloneBar.name = "HealthBar";
            cloneBar.gameObject.SetActive(true);

            RectTransform barRT =
                cloneBar.GetComponent<RectTransform>();

            cloneBar.transform.localScale = Vector3.one;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (cloneIcon != null)
        {
            // Seguir la posicion del cursor
            cloneIcon.transform.position = eventData.position;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (cloneIcon != null) {
            originalIcon.color = Color.white;
            Destroy(cloneIcon);
        }
    }

    public abstract void OnDrop(PointerEventData eventData);
}