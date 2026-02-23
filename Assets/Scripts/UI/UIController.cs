using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    // Definimos los posibles estados de la interfaz
    public enum UIState { Gameplay, Inventory, Pause }
    public UIState currentState = UIState.Gameplay;

    [Header("Canvas")]
    [SerializeField] private GameObject hudCanvas;
    [SerializeField] private GameObject inventoryCanvas;
    [SerializeField] private GameObject pauseCanvas;

    private InputAction inventoryAction;
    private InputAction pauseAction;

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
            if (currentState == UIState.Pause) SetState(UIState.Gameplay);
            else SetState(UIState.Pause);
        }
    }

    public void SetState(UIState newState)
    {
        currentState = newState;

        // Desactivamos todo por defecto y solo activamos el actual
        hudCanvas.SetActive(currentState == UIState.Gameplay);
        inventoryCanvas.SetActive(currentState == UIState.Inventory);
        pauseCanvas.SetActive(currentState == UIState.Pause);

        switch (currentState)
        {
            case UIState.Gameplay:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;

            case UIState.Inventory:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;

            case UIState.Pause:
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }
    }
}