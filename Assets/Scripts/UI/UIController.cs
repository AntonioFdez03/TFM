using UnityEngine;
using UnityEngine.InputSystem;

public enum UIState { Gameplay, Inventory, Pause, Crafting }
public class UIController : MonoBehaviour
{
    // Definimos los posibles estados de la interfaz
    public static UIState currentState = UIState.Gameplay;

    [Header("Canvas")]
    [SerializeField] private GameObject hudCanvas;
    [SerializeField] private GameObject inventoryCanvas;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject craftingCanvas;

    private InputAction inventoryAction;
    private InputAction interactAction;
    private InputAction exitAction;

    private static GameObject sHudCanvas;
    private static GameObject sInventoryCanvas;
    private static GameObject sPauseCanvas;
    private static GameObject sCraftingCanvas;

    void Start()
    {
        //Canvas
        sHudCanvas = hudCanvas;
        sInventoryCanvas = inventoryCanvas;
        sPauseCanvas = pauseCanvas;
        sCraftingCanvas = craftingCanvas;

        inventoryAction = InputSystem.actions.FindAction("Inventory");
        interactAction = InputSystem.actions.FindAction("Interact");
        exitAction = InputSystem.actions.FindAction("Exit");
        SetState(UIState.Gameplay);
    }

    void Update()
    {
        // Detectar cambio a Inventario
        if (inventoryAction.WasPressedThisFrame())
        {
            if (currentState == UIState.Inventory) SetState(UIState.Gameplay);
            else if (currentState == UIState.Gameplay) SetState(UIState.Inventory);
        }
        else if (exitAction.WasPressedThisFrame())
        {
            if (currentState == UIState.Gameplay) SetState(UIState.Pause);
            else SetState(UIState.Gameplay);
        }
    }

    public static void SetState(UIState newState)
    {
        currentState = newState;

        // Desactivamos todo por defecto y solo activamos el actual
        sHudCanvas.SetActive(currentState == UIState.Gameplay);
        sInventoryCanvas.SetActive(currentState == UIState.Inventory);
        sPauseCanvas.SetActive(currentState == UIState.Pause);
        sCraftingCanvas.SetActive(currentState == UIState.Crafting);

        switch (currentState)
        {
            case UIState.Gameplay:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                PlayerController.SetCanMove(true);
                break;

            case UIState.Inventory:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                PlayerController.SetCanMove(false);
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
                PlayerController.SetCanMove(false);
                break;

        }
    }
}