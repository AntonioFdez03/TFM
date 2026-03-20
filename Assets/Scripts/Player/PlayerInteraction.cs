using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ArmController arm;
    [SerializeField] TMP_Text itemInfo;
    [SerializeField] Material outlineMaterial;

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
                    hit.collider.gameObject.GetComponent<IInteractiveObject>().Interact();
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
        itemInfo.text = selected ? item.GetComponent<ItemData>().GetItemName() : "";
        foreach (Transform child in item.transform)
        {
            if(child.TryGetComponent(out MeshRenderer meshRenderer)){
                List<Material> itemMaterials = new List<Material>(meshRenderer.materials);
                if(selected && meshRenderer.materials.Length < 2)
                {
                    //itemMaterials.Add(outlineMaterial);
                }
                else if(!selected && meshRenderer.materials.Length > 1)
                    itemMaterials.RemoveAll(m => m.name.Contains(outlineMaterial.name));
                meshRenderer.materials = itemMaterials.ToArray();
            }
        }
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
