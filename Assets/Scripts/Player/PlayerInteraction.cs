using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ArmController arm;
    [SerializeField] InventoryController inventory;
    private float interactDistance = 10f;
    private LayerMask interactLayer = 1;

    private InputAction interact;
    private InputAction attack;

    void Start()
    {
        interact = InputSystem.actions.FindAction("Interact");
        attack = InputSystem.actions.FindAction("Attack");
    }

    void Update()
    {
        if (PlayerController.playerInstance.GetCanMove())
        {
            Interact();
            Attack();
        }
    }

    private void Interact()
    {
        // Rayo desde el centro de la cámara
        Ray ray = new Ray(CameraController.playerCameraInstance.transform.position, CameraController.playerCameraInstance.transform.forward);
        RaycastHit hit;

        //Muestra el rayo en el editor
        //Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.blue);

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer) && interact.WasPressedThisFrame())
        {
            if (hit.collider.CompareTag("Item"))  
                inventory.AddItem(hit.collider.gameObject);
            else if(hit.collider.CompareTag("Interactive"))
                hit.collider.gameObject.GetComponent<InteractiveObject>().Interact();
        }
    }

    private void Attack()
    {
        if (attack.triggered)
        {   
            if (arm != null) 
                arm.PlayAttackAnimation();
        }
    }
}
