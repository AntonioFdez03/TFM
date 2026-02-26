using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;

    [Header("References")]
    [SerializeField] Transform gridPanel; 
    [SerializeField] Transform hotBarPanel;
    [SerializeField] Transform hudHotBar;
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Transform DragginLayer;
    
    [Header("Settings")]
    [SerializeField] int gridSize = 21;
    [SerializeField] int hotBarSize = 7;

    private List<Image> inventorySlots = new List<Image>();

    void Awake()
    {   
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        CleanPanel(hudHotBar);
        CleanPanel(hotBarPanel);
        CleanPanel(gridPanel);

        GenerateSlots(hudHotBar, hotBarSize, 0);
        GenerateSlots(hotBarPanel, hotBarSize, 0); // Barra de equipamiento del inventario
        GenerateSlots(gridPanel, gridSize, hotBarSize); // Grid del inventario
    }

    void CleanPanel(Transform panel)
    {   
        foreach(Transform child in panel)
        {
            Destroy(child.gameObject);
        }
    }
    
    void GenerateSlots(Transform parent, int count, int startIndex)
    {
        for (int i = startIndex; i < count + startIndex; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, parent);
            newSlot.name = "Slot_" + i;

            RectTransform rect = newSlot.GetComponent<RectTransform>();
            if (rect != null) rect.sizeDelta = new Vector2(110f,110f);
        

            InventorySlot scriptSlot = newSlot.GetComponent<InventorySlot>();
            if (scriptSlot != null)
            {
                scriptSlot.slotIndex = i;
                scriptSlot.SetDragginLayer(DragginLayer);
            }

            inventorySlots.Add(newSlot.GetComponent<Image>());
            CreateChildIcon(newSlot.transform, i);
        }
    }

    //Función para crear un hijo imagen para cada slot, que contiene el icono del objeto
    void CreateChildIcon(Transform slot, int index)
    {
        GameObject itemIcon = new GameObject("Icon_" + index);
        itemIcon.transform.SetParent(slot);
        
        // Añadimos la imagen para el sprite
        Image iconImage = itemIcon.AddComponent<Image>();
        iconImage.raycastTarget = false; 

        // --- EL CAMBIO CLAVE: Añadir ItemData ---
        itemIcon.AddComponent<ItemData>(); 
        // ----------------------------------------
        
        RectTransform rect = itemIcon.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;

        itemIcon.SetActive(false);
    }

    public void UpdateUI()
    {
        GameObject[] items = InventoryController.inventoryInstance.GetInventoryItems();

        foreach (Image currentSlot in inventorySlots)
        {
            InventorySlot scriptSlot = currentSlot.GetComponent<InventorySlot>();
            
            if (scriptSlot != null)
            {
                int realIndex = scriptSlot.slotIndex; 
                
                if (currentSlot.transform.childCount > 0)
                {
                    GameObject slotItem = currentSlot.transform.GetChild(0).gameObject;

                    if (realIndex < items.Length && items[realIndex] != null)
                    {
                        // 1. Obtenemos los datos del objeto real que está en el controlador
                        ItemData originalData = items[realIndex].GetComponent<ItemData>();
                        
                        // 2. Obtenemos el script ItemData que añadimos al Icono en CreateChildIcon
                        ItemData uiData = slotItem.GetComponent<ItemData>();

                        if (originalData != null && uiData != null)
                        {
                            // 3. PASO CRUCIAL: Copiamos la información al icono de la UI
                            uiData.CopyFrom(originalData);

                            // 4. Actualizamos el aspecto visual
                            slotItem.GetComponent<Image>().sprite = originalData.GetItemIcon();
                            slotItem.SetActive(true);
                        }
                    }
                    else
                    {
                        // Si el slot está vacío, reseteamos el ItemData de la UI para no dejar basura
                        ItemData uiData = slotItem.GetComponentInChildren<ItemData>();
                        if (uiData != null) uiData.SetItemPrefab(null);

                        slotItem.SetActive(false);
                    }
                }
            }
        }
    }
}