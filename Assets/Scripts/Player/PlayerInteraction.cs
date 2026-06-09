using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ArmController arm;
    [SerializeField] Material outlineMaterial;

    private float interactDistance = 9f;
    private ItemStack previousItem;
    private InputAction interact;
    private InputAction attack;
    private InputAction rotate;
    private InputAction toggleActivation;
    private InputAction aim;
    private RaycastHit lastHit;

    private float interactTime = 0.2f;
    private float timer;

    void Start()
    {   
        interact = InputSystem.actions.FindAction("Interact");
        attack = InputSystem.actions.FindAction("Attack");
        rotate = InputSystem.actions.FindAction("Rotate");
        toggleActivation = InputSystem.actions.FindAction("ToggleActivation");
        aim = InputSystem.actions.FindAction("Aim");
    }

    void Update()
    {
        GameplayUI.instance.ClearKeys();

        if (PlayerController.instance.GetCanMove())
        {
            Interact();
            Use();
            HandleActivateable(HotBarController.instance.GetHandItem());
            Aim();
        }

        var current = HotBarController.instance.GetCurrentItem();

        if (current != previousItem)
        {
            CancelUse();
            previousItem = current;
        }

        ItemBehaviour itemBehaviour = HotBarController.instance.GetCurrentItemBehaviour();
        GameObject handItem = HotBarController.instance.GetHandItem();

        if(itemBehaviour == null)
            GameplayUI.instance.AddKey("RMB", "Punch");
        else if(itemBehaviour is EquipmentBehaviour)
            GameplayUI.instance.AddKey("RMB", "Attack");

        if(handItem != null)
        {
            if(handItem.TryGetComponent(out IActivateableObject activateableObject))
            {   
                GameplayUI.instance.AddKey("F", activateableObject.isActive() ? "Desactivate" : "Activate");
            }
        }

        if(InventoryController.instance.CanDrop(HotBarController.instance.GetSelectedIndex()))
            GameplayUI.instance.AddKey("Q", "Drop item");

    }

    private void Use()
    {
        var currentItem = HotBarController.instance.GetCurrentItemBehaviour();
        HandleItemUses(currentItem);
    }

    private void CancelUse()
    {
        GameplayUI.instance.HideCircularSlider();

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

        int layerMask = ~LayerMask.GetMask("Light");

        bool hasHit = Physics.Raycast(
            ray,
            out hit,
            interactDistance,
            layerMask
        );

        GameObject hitObject = hasHit ? hit.collider.gameObject : null;
        string tag = hasHit ? hit.collider.tag : null;

        

        HandleHover(hasHit,hitObject,tag);
        HandlePlaceableSilhouette(hasHit, hit);

        if (!hasHit)
        {   
            GameplayUI.instance.HideCircularSlider();
            GameplayUI.instance.HideItemHealth();
            GameplayUI.instance.HideItemName();
            return;
        }

        if(hit.collider.CompareTag("Terrain"))
            GameplayUI.instance.HideItemName();
        
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
            GameplayUI.instance.AddKey("HoldRMB", "Consume");
            if (attack.IsPressed() && consumable != null)
            {
                consumable.Use();
                GameplayUI.instance.ShowCircularSlider(consumable.GetCurrentTime() / consumable.GetConsumeTime(), false);
            }
            else
                ResetTime(consumable);
        }
    }

    public void ResetTime(ItemBehaviour obj)
    {   
         GameplayUI.instance.HideCircularSlider();

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
                    if (InventoryController.instance.CanAdd())
                    {
                        if(hitObject.GetComponent<PlaceableBehaviour>() != null)
                            GameplayUI.instance.AddKey("HoldE", "Unplace");
                        else
                            GameplayUI.instance.AddKey("E", "Pick up");
                    }
                    break;

                case "Harvestable":
                    HandleHarvestableInfo(hitObject);
                    if(hitObject.GetComponent<Bush>() != null)
                        GameplayUI.instance.AddKey("HoldE", "Recolect");
                    break;

                case "Interactive":
                    HandleItemSelection(hitObject, true);
                    GameplayUI.instance.AddKey("E", "Interact");
                    if(hitObject.GetComponent<PlaceableBehaviour>() != null)
                        GameplayUI.instance.AddKey("HoldE", "Unplace");
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
                    return;

                case "Interactive":
                    HandleItemSelection(item, false);
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
            GameplayUI.instance.ShowCircularSlider(placeable.GetCurrentTime() / placeable.GetUnplaceTime(), true);
            return;
        }
        else
             ResetTime(placeable);

        if(interact.IsPressed() && bush != null)
        {   
            bush.Recolect();
            GameplayUI.instance.ShowCircularSlider(bush.GetCurrentTime() / bush.GetRecolectTime(), true);
            return;
        }
        else
        {   
            GameplayUI.instance.HideCircularSlider();
            
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
            GameplayUI.instance.AddKey("RMB", "Place");
            GameplayUI.instance.AddKey("R", "Rotate");
            placeable.ShowSilhouette(hit);
            if(rotate.IsPressed())
                placeable.RotateSilhouette();
        }
        else
            placeable.HideSilhouette();
    }

    private void HandleItemSelection(GameObject item, bool selected)
    {   
        if(item.TryGetComponent(out ItemBehaviour itemB))
        {   
            if(selected)
                GameplayUI.instance.ShowItemName(itemB.GetData().itemName);
            else
                GameplayUI.instance.HideItemName();
        }

        if(item.TryGetComponent<IObjectHealth>(out var objectHealth) && objectHealth.GetCurrentHealth() != objectHealth.GetMaxHealth())  
            GameplayUI.instance.ShowItemHealth(objectHealth.GetCurrentHealth(), objectHealth.GetMaxHealth());
        else
            GameplayUI.instance.HideItemHealth();
        
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
            //string article = "a";
            if ("AEIOU".IndexOf(firstLetter) >= 0){
                //article = "an";
            }
            if(HotBarController.instance.GetCurrentItemBehaviour() is ToolBehaviour toolBehaviour)
            {
                if (!harvestable.CanHarvest(toolBehaviour.GetToolType()))
                {
                    //itemName.text = $"You need {article} {toolName}";
                }
            }
            else
            {
                //itemName.text = $"You need {article} {toolName}";
            }    
        }
    }

    private void HandleActivateable(GameObject item)
    {   
        if(item == null) return;

        print("Item: " + item);
        if (item.TryGetComponent(out IActivateableObject activateable) && toggleActivation.WasPressedThisFrame())
        {   
            print("Entra");
            activateable.ToggleActivation();
        }
    }

    private void Aim()
    {   
        GameObject item = HotBarController.instance.GetHandItem();
        
        if(item == null) return;

        if(item.TryGetComponent(out IAim aimItem))
        {   
            if(aim.IsPressed())
                aimItem.Aim();
            else if(aim.WasReleasedThisFrame())
                aimItem.Shoot();
        }

    }
}
