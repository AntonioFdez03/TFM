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
    private RaycastHit lastHit;

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

        if(lastHit.collider != null)
            SetChildrenLayer(lastHit.collider.gameObject, LayerMask.NameToLayer("Default"));

        bool hasHit = Physics.Raycast(ray, out hit, interactDistance);

        if(hasHit)
        {
            if(hit.collider.CompareTag("Item"))
                SetChildrenLayer(hit.collider.gameObject, LayerMask.NameToLayer("Outline"));
        }

        if (HotBarController.instance.GetCurrentItemBehaviour() is PlaceableBehaviour placeable)
        {
            if (hasHit && hit.collider.CompareTag("Terrain"))
                placeable.ShowSilhouette(hit);
            else if (hasHit == false)
                placeable.HideSilhouette();
        }

        if (!hasHit)
            return;

        if (interact.WasPressedThisFrame())
        {
            if (hit.collider.CompareTag("Item"))
                inventory.AddItem(hit.collider.gameObject);
            else if (hit.collider.CompareTag("Interactive"))
                hit.collider.gameObject.GetComponent<InteractiveObject>().Interact();
        }

        lastHit = hit;
    }

    private void Attack()
    {
        if (attack.triggered)
        {   
            if (arm != null) 
                arm.PlayAttackAnimation();

            if(HotBarController.instance.GetCurrentItemBehaviour() is PlaceableBehaviour placeable)
                placeable.Use();
        }
    }

    private void SetChildrenLayer(GameObject item, LayerMask layer)
    {   
        print("Layer");
        item.layer = layer;

        foreach (Transform child in item.transform)
            SetChildrenLayer(child.gameObject, layer);
    }
}
