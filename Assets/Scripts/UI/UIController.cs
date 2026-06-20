using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum UIState { None, Gameplay, Inventory, Pause, Settings, Crafting, Furnace, Storage }
public class UIController : MonoBehaviour
{
    public static UIController instance;
    private UIState currentState = UIState.Gameplay;

    [Header("Canvas")]
    [SerializeField] private GameObject hudCanvas;
    [SerializeField] private GameObject inventoryCanvas;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject mixCanvas;
    [SerializeField] private GameObject craftPanel;
    [SerializeField] private GameObject furnacePanel;
    [SerializeField] private GameObject storagePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Transform dragginLayer;

    [Header("Sounds")]
    [SerializeField] private AudioClip inventoryOpen;
    [SerializeField] private AudioClip inventoryClose;

    private InputAction inventoryAction;
    private InputAction pauseAction;
    private InputAction craftAction;
    private Furnace currentFurnace;
    private Storage currentStorage;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        inventoryAction = InputSystem.actions.FindAction("Inventory");
        pauseAction = InputSystem.actions.FindAction("Pause");
        craftAction = InputSystem.actions.FindAction("Craft");
        SetState(UIState.Gameplay);
    }

    public UIState GetCurrentState() => currentState;

    void Update()
    {   
        // Detectar cambio a Inventario
        if (inventoryAction.WasPressedThisFrame())
        {
            if (currentState == UIState.Inventory) SetState(UIState.Gameplay);
            else if (currentState == UIState.Gameplay) SetState(UIState.Inventory);
        }

        // Detectar cambio a Pausa
        if (pauseAction.WasPressedThisFrame())
        {
            if (currentState == UIState.Gameplay) SetState(UIState.Pause);
            else SetState(UIState.Gameplay);
        }

        // Craft
        if (craftAction.WasPressedThisFrame())
        {
            if(currentState == UIState.Gameplay || currentState == UIState.Inventory) SetState(UIState.Crafting);
            else if(currentState == UIState.Crafting) SetState(UIState.Inventory);        
        }
    }

    public void SetState(UIState newState)
    {
        currentState = newState;

        // Desactivamos todo por defecto y solo activamos el actual
        hudCanvas.SetActive(currentState == UIState.Gameplay);
        inventoryCanvas.SetActive(currentState == UIState.Inventory);
        pauseCanvas.SetActive(currentState == UIState.Pause);
        mixCanvas.SetActive(currentState == UIState.Crafting || currentState == UIState.Furnace || currentState == UIState.Storage);
        settingsPanel.SetActive(currentState == UIState.Settings);

        if (currentState == UIState.Storage && newState != UIState.Storage)
            currentStorage = null;
        
        switch (currentState)
        {
            case UIState.Gameplay:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                PlayerController.instance.SetCanMove(true);
                break;

            case UIState.Inventory:
                AudioManager.instance.PlayOneShot(inventoryOpen, 0.2f);
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                CraftingController.instance.SetStationType(CraftingStationType.None);
                PlayerController.instance.SetCanMove(false);
                break;

            case UIState.Pause:
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            
            case UIState.Settings:
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;

            case UIState.Crafting:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                LoadPanel(craftPanel);

                PlayerController.instance.SetCanMove(false);
                break;
            
            case UIState.Furnace:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                LoadPanel(furnacePanel);
                PlayerController.instance.SetCanMove(false);
                break;

            case UIState.Storage:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                LoadPanel(storagePanel);
                PlayerController.instance.SetCanMove(false);
                break;

        }
    }

    private void LoadPanel(GameObject panel)
    {
        print("Panel: " + panel);
        Transform panelsCanvas = mixCanvas.transform.GetChild(0);

        if (panelsCanvas.childCount > 1)
            Destroy(panelsCanvas.GetChild(1).gameObject);

        GameObject panelInstance = Instantiate(panel, panelsCanvas);
        panelInstance.transform.localScale = Vector3.one;

        switch (currentState)
        {
            case UIState.Furnace:
                FurnaceUI furnaceUI = panelInstance.GetComponent<FurnaceUI>();
                if (furnaceUI != null)
                {
                    furnaceUI.SetDragginLayer(dragginLayer);
                    furnaceUI.SetFurnace(currentFurnace);
                }
                break;
            case UIState.Storage:
                StorageUI storageUI = panelInstance.GetComponent<StorageUI>();
                    if (storageUI != null)
                    {
                        storageUI.SetDragginLayer(dragginLayer);
                        storageUI.SetStorage(currentStorage);
                    }
                break;
        }
    }

    public void OpenFurnace(Furnace furnace)
    {
        currentFurnace = furnace;
        SetState(UIState.Furnace);
    }

    public void OpenStorage(Storage storage)
    {
        currentStorage = storage;
        SetState(UIState.Storage);
    }

    public Storage GetCurrentStorage() => currentStorage;
    public Furnace GetCurrentFurnace() => currentFurnace;
    public void SetCraftingState() => SetState(UIState.Crafting);
    public void SetInventoryState() => SetState(UIState.Inventory);
}