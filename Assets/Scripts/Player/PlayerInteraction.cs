using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ArmController arm;
    [SerializeField] TMP_Text itemInfo;
    [SerializeField] Material outlineMaterial;
    [SerializeField] Slider circularSlider;

    private float interactDistance = 9f;

    private InputAction interact;
    private InputAction attack;
    private RaycastHit lastHit;

    private float interactTime = 0.2f;
    private float timer;

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
            Use();
        }
    }

    private void Use()
    {
        var currentItem = HotBarController.instance.GetCurrentItemBehaviour();

        HandleAttackStart(currentItem);
        HandleConsumable(currentItem);
    }

    private void Interact()
    {
        Ray ray = new Ray(CameraController.instance.transform.position, CameraController.instance.transform.forward);
        RaycastHit hit;

        // Limpiar selección anterior
        if (lastHit.collider != null)
            HandleItemSelection(lastHit.collider.gameObject, false);

        bool hasHit = Physics.Raycast(ray, out hit, interactDistance);

        GameObject hitObject = hasHit ? hit.collider.gameObject : null;
        string tag = hasHit ? hit.collider.tag : null;

        HandleHover(hasHit,hitObject,tag);
        HandlePlaceableSilhouette(hasHit, hit);

        if (!hasHit)
            return;
        
        HandleInteraction(tag,hitObject);
        
        lastHit = hit;
    }

    private void HandleAttackStart(object currentItem)
    {
        if (!attack.triggered)
            return;

        if (arm != null)
            arm.PlayAttackAnimation();

        if (currentItem is PlaceableBehaviour placeable)
            placeable.Use();
    }

    private void HandleConsumable(object currentItem)
    {
        if(currentItem is ConsumableBehaviour consumable)
        {
            if (attack.IsPressed() && consumable != null)
            {
                consumable.Use();
                ShowCircularSlider(consumable.GetCurrentTime() / consumable.GetConsumeTime(),false);
            }
            else
                ResetTime(consumable);
        }
    }

    private void ResetTime(ItemBehaviour obj)
    {   
        if (obj == null)
        {
            circularSlider.transform.parent.gameObject.SetActive(false);
            return;
        }

        if (obj.TryGetComponent<ConsumableBehaviour>(out var consumable))
            consumable.SetCurrentTime(0f);

        if (obj.TryGetComponent<PlaceableBehaviour>(out var placeable))
            placeable.SetCurrentTime(0f);

        circularSlider.transform.parent.gameObject.SetActive(false);
    }
    
    private void HandleHover(bool hasHit, GameObject hitObject, string tag)
    {
        if (hasHit)
        {
            switch (tag)
            {
                case "Item":
                    HandleItemSelection(hitObject, true);
                    break;

                case "Harvestable":
                    HandleHarvestableInfo(hitObject);
                    break;
            }
        }
    }

    private void HandleInteraction(string tag, GameObject obj)
    {   
        if (interact.WasReleasedThisFrame() && timer < interactTime) 
        {   
            timer = 0;
            ResetTime(obj.GetComponent<ItemBehaviour>());
            switch (tag)
            {
                case "Item":
                    HandleItemSelection(obj, false);
                    InventoryController.instance.AddItem(obj);
                    break;

                case "Interactive":
                    obj.GetComponent<IInteractiveObject>()?.Interact();
                    break;
            }
        }else if (interact.IsPressed())    
            timer += Time.deltaTime;
        else
            timer = 0;

        HandleUnplacing(obj);
    }

    private void HandleUnplacing(GameObject obj)
    {   
        if(obj.GetComponent<ItemBehaviour>() is PlaceableBehaviour placeable)
        {   
            if(interact.IsPressed() && placeable != null)
            {   
                placeable.Unplace();
                ShowCircularSlider(placeable.GetCurrentTime() / placeable.GetUnplaceTime(), true);
            }
            else
                ResetTime(placeable);
            
        }
    }

    private void HandlePlaceableSilhouette(bool hasHit, RaycastHit hit)
    {
        if (HotBarController.instance.GetCurrentItemBehaviour() is not PlaceableBehaviour placeable)
            return;

        if (hasHit && hit.collider.CompareTag("Terrain"))
            placeable.ShowSilhouette(hit);
        else
            placeable.HideSilhouette();
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
            string toolName = harvestable.GetToolsAccepted()[0].ToString();
            char firstLetter = char.ToUpper(toolName[0]);
            string article = "a";
            if ("AEIOU".IndexOf(firstLetter) >= 0)
                article = "an";
            if(HotBarController.instance.GetCurrentItemBehaviour() is ToolBehaviour toolBehaviour)
            {   
                if(!harvestable.CanHarvest(toolBehaviour.GetToolType()))
                    itemInfo.text = $"You need {article} {toolName}";
            }else
                itemInfo.text = $"You need {article} {toolName}";
        }
    }

    private void ShowCircularSlider(float currentValue, bool delay)
    {   

        float startTime = 0;
        if(delay) startTime = 0.2f;

        if(currentValue > startTime)
        {   
            circularSlider.transform.parent.gameObject.SetActive(true);
            circularSlider.value = currentValue;
        }
        else
             circularSlider.transform.parent.gameObject.SetActive(false);
    }
}
