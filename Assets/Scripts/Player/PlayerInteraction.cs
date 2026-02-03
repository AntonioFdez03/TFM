using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] InventoryController inventory;

    [Header("Settings")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float interactDistance = 100f;
    [SerializeField] private LayerMask interactLayer = 0;

    private InputAction interact;

    void Start()
    {
        interact = InputSystem.actions.FindAction("Interact");
    }

    void Update()
    {   
        CheckForInteractable();
    }

    private void CheckForInteractable()
    {
        // Rayo desde el centro de la cámara
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        //Muestra el rayo en el editor
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.blue);

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            if (interact.WasPressedThisFrame())
            {   
                inventory.AddItem(hit.collider.gameObject);
            }
        }
    }
}
