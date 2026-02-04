using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ArmController arm;
    [SerializeField] InventoryController inventory;
    [SerializeField] HotBarController hotBarController;
    [SerializeField] private Transform playerCamera;
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
        Interact();
        Attack();
    }

    private void Interact()
    {
        // Rayo desde el centro de la cámara
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        //Muestra el rayo en el editor
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.blue);

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            if (interact.WasPressedThisFrame() && hit.collider.CompareTag("Item"))
            {   
                inventory.AddItem(hit.collider.gameObject);
            }
        }
    }

    private void Attack()
    {
        print("Entra");
        if (attack.WasPressedThisFrame())
        {   
            ItemBehaviour item = hotBarController.GetCurrentItemBehaviour();
            if(item is ToolBehaviour || item == null)
            {
                if (arm != null) 
                    arm.PlayAttackAnimation();
                if (item != null)
                {
                    item.Use();
                    print("Usando item: " + item.GetItemData().GetItemName());
                }
            }
            else
            {
                print("Otro tipo de item");
            }
        }
    }
}
