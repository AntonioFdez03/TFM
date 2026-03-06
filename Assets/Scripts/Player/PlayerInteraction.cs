using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ArmController arm;
    [SerializeField] InventoryController inventory;
    private float interactDistance = 9f;

    private InputAction interact;
    private InputAction attack;

    void Start()
    {
        interact = InputSystem.actions.FindAction("Interact");
        attack = InputSystem.actions.FindAction("Attack");
    }

    void Update()
    {
        if (PlayerController.instance.GetCanMove())
        {
            Interact();
            Attack();
        }
    }

    private void Interact()
    {
        Ray ray = new Ray(CameraController.instance.transform.position, CameraController.instance.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if(interact.WasPressedThisFrame())
            {
                if (hit.collider.CompareTag("Item"))  
                    inventory.AddItem(hit.collider.gameObject);
                else if(hit.collider.CompareTag("Interactive"))
                    hit.collider.gameObject.GetComponent<InteractiveObject>().Interact();
            }

            if(hit.collider.CompareTag("Terrain") && HotBarController.instance.GetCurrentItemBehaviour() is PlaceableBehaviour)
                HotBarController.instance.GetCurrentItem().GetComponent<PlaceableBehaviour>().ShowSilhouette(hit);
                
        }
    }

    private void Attack()
    {
        if (attack.triggered)
        {   

            print("Current behaviour: " + HotBarController.instance.GetCurrentItemBehaviour());
            if (arm != null) 
                arm.PlayAttackAnimation();

            if(HotBarController.instance.GetCurrentItemBehaviour() is PlaceableBehaviour)
                HotBarController.instance.GetCurrentItem().GetComponent<PlaceableBehaviour>().Use();
        }
    }
}
