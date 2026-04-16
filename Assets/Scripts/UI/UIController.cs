using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum UIState { None, Gameplay, Inventory, Pause, Crafting, Furnace }
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

    private InputAction inventoryAction;
    private InputAction pauseAction;

    private UIState lastPanel = UIState.None;
    private Furnace currentFurnace;

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
        SetState(UIState.Gameplay);
    }

    public UIState GetCurrentState() => currentState;

    void Update()
    {   
        if(PlayerController.instance.IsDead())
            SceneManager.LoadScene("MainMenuScene");

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
    }

    public void SetState(UIState newState)
    {
        currentState = newState;

        // Desactivamos todo por defecto y solo activamos el actual
        hudCanvas.SetActive(currentState == UIState.Gameplay);
        inventoryCanvas.SetActive(currentState == UIState.Inventory);
        pauseCanvas.SetActive(currentState == UIState.Pause);
        mixCanvas.SetActive(currentState == UIState.Crafting || currentState == UIState.Furnace);

        switch (currentState)
        {
            case UIState.Gameplay:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                PlayerController.instance.SetCanMove(true);
                break;

            case UIState.Inventory:
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
                
            case UIState.Crafting:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                LoadPanel(currentState,craftPanel);

                PlayerController.instance.SetCanMove(false);
                lastPanel = UIState.Crafting;
                break;
            
            case UIState.Furnace:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                LoadPanel(currentState,furnacePanel);
                PlayerController.instance.SetCanMove(false);
                lastPanel = UIState.Furnace;
                break;
        }
    }

    private void LoadPanel(UIState state, GameObject panel)
    {
        Transform panelsCanvas = mixCanvas.transform.GetChild(0);
        if(panelsCanvas.childCount > 1)
        {
            if(lastPanel == state)
                return;
            else
                Destroy(panelsCanvas.GetChild(1).gameObject);
        }
        
        GameObject panelInstance = Instantiate(panel);
        panelInstance.transform.SetParent(mixCanvas.transform.GetChild(0));
        panelInstance.transform.localScale = Vector3.one;

        switch (state)
        {   
            case UIState.Furnace:
                FurnaceUI furnaceUI = panelInstance.GetComponentInChildren<FurnaceUI>();

                if (furnaceUI != null)
                {   
                    furnaceUI.SetFurnace(currentFurnace);
                }
            break;
        }    
    }

    public void OpenFurnace(Furnace furnace)
    {
        currentFurnace = furnace;
        SetState(UIState.Furnace);
    }

    public void SetCraftingState() => SetState(UIState.Crafting);
    public void SetInventoryState() => SetState(UIState.Inventory);
}