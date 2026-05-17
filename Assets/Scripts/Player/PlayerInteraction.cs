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
    [SerializeField] Material outlineMaterial;
    [SerializeField] Slider circularSlider;
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text itemHealth;
    [SerializeField] Image itemHealthSlider;

    private float interactDistance = 9f;
    private ItemStack previousItem;
    private InputAction interact;
    private InputAction attack;
    private InputAction rotate;
    private RaycastHit lastHit;

    private float interactTime = 0.2f;
    private float timer;

    void Start()
    {
        interact = InputSystem.actions.FindAction("Interact");
        attack = InputSystem.actions.FindAction("Attack");
        rotate = InputSystem.actions.FindAction("Rotate");
    }

    void Update()
    {
        if (PlayerController.instance.GetCanMove())
        {
            Interact();
            Use();
        }

        var current = HotBarController.instance.GetCurrentItem();

        if (current != previousItem)
        {
            CancelUse();
            previousItem = current;
        }
    }

    private void Use()
    {
        var currentItem = HotBarController.instance.GetCurrentItemBehaviour();
        HandleItemUses(currentItem);
    }

    private void CancelUse()
    {
        circularSlider.transform.parent.gameObject.SetActive(false);

        // Si había un consumible activo, resetea su progreso
        if (previousItem != null)
        {
            var behaviour = HotBarController.instance.GetCurrentItemBehaviour();

            if (behaviour is ConsumableBehaviour consumable)
                consumable.SetCurrentTime(0f);

            if (behaviour is PlaceableBehaviour placeable)
                placeable.SetCurrentTime(0f);
        }
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
        {   
            circularSlider.transform.parent.gameObject.SetActive(false);
            itemHealth.transform.parent.gameObject.SetActive(false);
            itemName.text = "";
            return;
        }

        if(hit.collider.CompareTag("Terrain"))
            itemName.text = "";
        
        HandleInteraction(tag,hitObject);
        lastHit = hit;
    }

    private void HandleItemUses(object currentItem)
    {
        if (attack.triggered && UIController.instance.GetCurrentState() != UIState.Pause)
        {   
            if (arm != null) 
                arm.PlayAttackAnimation();

            if(HotBarController.instance.GetCurrentItemBehaviour() is PlaceableBehaviour placeable)
                placeable.Use();
        }

        //Consumable
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


    public void ResetTime(ItemBehaviour obj)
    {   
        circularSlider.transform.parent.gameObject.SetActive(false);

        if (obj == null)
            return;

        if (obj.TryGetComponent<ConsumableBehaviour>(out var consumable))
            consumable.SetCurrentTime(0f);

        if (obj.TryGetComponent<PlaceableBehaviour>(out var placeable))
            placeable.SetCurrentTime(0f);


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

    private void HandleInteraction(string tag, GameObject item)
    {   
        if (interact.WasReleasedThisFrame() && timer < interactTime) 
        {   
            timer = 0;
            ResetTime(item.GetComponent<ItemBehaviour>());
            switch (tag)
            {
                case "Item":
                    HandleItemSelection(item, false);
                    InventoryController.instance.AddItem(item);
                    break;

                case "Interactive":
                    if (item.TryGetComponent(out IInteractiveObject interactiveObject))
                        interactiveObject.Interact();
                    else
                        item.GetComponentInParent<IInteractiveObject>()?.Interact();
                    break;
            }
        }else if (interact.IsPressed())    
            timer += Time.deltaTime;
        else
            timer = 0;

        HandleInteractHolding(item);
    }

    private void HandleInteractHolding(GameObject obj)
    {   
        PlaceableBehaviour placeable = obj.GetComponent<ItemBehaviour>() as PlaceableBehaviour; 
        Bush bush = obj.GetComponent<Bush>();

        if(interact.IsPressed() && placeable != null)
        {   
            placeable.Unplace();
            ShowCircularSlider(placeable.GetCurrentTime() / placeable.GetUnplaceTime(), true);
        }
        else
             ResetTime(placeable);

        if(interact.IsPressed() && bush != null)
        {   
            bush.Recolect();
            ShowCircularSlider(bush.GetCurrentTime() / bush.GetRecolectTime(), true);
        }
        else
        {   
            circularSlider.transform.parent.gameObject.SetActive(false);
            if(bush != null)
                bush.SetCurrentTime(0);
        }
            
    }

    private void HandlePlaceableSilhouette(bool hasHit, RaycastHit hit)
    {
        if (HotBarController.instance.GetCurrentItemBehaviour() is not PlaceableBehaviour placeable)
            return;

        if (hasHit && hit.collider.CompareTag("Terrain"))
        {
            placeable.ShowSilhouette(hit);
            if(rotate.IsPressed())
                placeable.RotateSilhouette();
        }
        else
            placeable.HideSilhouette();
    }

    private void HandleItemSelection(GameObject item, bool selected)
    {   
        Debug.Log("item: " + item);
        if(item.TryGetComponent(out ItemBehaviour itemB))
            itemName.text = selected ? itemB.GetData().itemName : "";
        Debug.Log("Primer if: " + item);
        if(item.TryGetComponent<ItemBehaviour>(out var itemBehaviour) && itemBehaviour.GetCurrentHealth() != itemBehaviour.GetMaxHealth())
        {   
            itemHealth.text = itemBehaviour.GetCurrentHealth().ToString() + "/" + itemBehaviour.GetMaxHealth().ToString();
            itemHealthSlider.fillAmount = itemBehaviour.GetCurrentHealth()/itemBehaviour.GetMaxHealth();
                
            itemHealth.transform.parent.gameObject.SetActive(true);
        }
        else
            itemHealth.transform.parent.gameObject.SetActive(false);
        
        Debug.Log("Segundo if: " + item);
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
                    itemName.text = $"You need {article} {toolName}";
            }else
                itemName.text = $"You need {article} {toolName}";
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
        else{
            circularSlider.transform.parent.gameObject.SetActive(false);
        }
    }
}
