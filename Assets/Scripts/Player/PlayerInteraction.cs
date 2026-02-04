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
    private float attackDistance = 5f;
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
            if (interact.WasPressedThisFrame())
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
            if (arm != null) arm.PlayAttackAnimation();
            ItemBehaviour item = hotBarController.GetCurrentItemBehaviour();
            print(item);
            if (item != null)
            {
                item.Use();
            }
        }
    }
}
