using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ArmController arm;
    [SerializeField] TMP_Text itemInfo;

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
            HandleItemSelection(lastHit.collider.gameObject, false);

        bool hasHit = Physics.Raycast(ray, out hit, interactDistance);

        if(hasHit)
        {
            switch (hit.collider.tag)
            {
                case "Item":
                    HandleItemSelection(hit.collider.gameObject, true);
                    break;

                case "Harvestable":
                    HandleHarvestableInfo(hit.collider.gameObject);
                    break;
            }
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
            switch (hit.collider.tag)
            {
                case "Item":
                    HandleItemSelection(hit.collider.gameObject, false);
                    InventoryController.instance.AddItem(hit.collider.gameObject);
                    break;

                case "Interactive":
                    hit.collider.gameObject.GetComponent<InteractiveObject>().Interact();
                    break;
            }
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

    private void HandleItemSelection(GameObject item, bool selected)
    {   
        item.layer = selected ? LayerMask.NameToLayer("Outline") : LayerMask.NameToLayer("Default");
        
        if(selected){
            if(item.TryGetComponent(out ItemData data))
                itemInfo.text = data.GetItemName();
        }
        else
            itemInfo.text = "";

        foreach (Transform child in item.transform)
            HandleItemSelection(child.gameObject, selected);
    }

    private void HandleHarvestableInfo(GameObject harvestableObject)
    {
        if(harvestableObject.TryGetComponent(out HarvestableObject harvestable))
        {
            if(HotBarController.instance.GetCurrentItemBehaviour() is ToolBehaviour toolBehaviour)
            {   
                if(!harvestable.CanHarvest(toolBehaviour.GetToolType()))
                    itemInfo.text = "You need an " + harvestable.GetToolsAccepted(); 
            }else
                itemInfo.text = "You need an " + harvestable.GetToolsAccepted()[0].ToString(); 
        }
    }
}
