using System;
using System.Collections;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceableBehaviour : ItemBehaviour
{
    [SerializeField] Material greenMaterial;
    [SerializeField] Material redMaterial;
    [SerializeField] LayerMask placementMask;

    protected Vector3 checkBoxSize;
    private GameObject silhouette;
    private Vector3 lastValidPosition;
    private Quaternion lastValidRotation;
    private bool canPlace;
    protected bool canUnplace = true;
    protected bool straight = false;

    private InputAction interact;
    protected float unplaceTime = 1f;
    private float timer;

    protected virtual void Start()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        interact = InputSystem.actions.FindAction("Interact");
    }

    protected void SetCheckBoxSize(Vector3 boxSize) => checkBoxSize = boxSize; 
    public float GetCurrentTime() => timer; 
    public float SetCurrentTime(float time) => timer = time; 
    public float GetUnplaceTime() => unplaceTime;
    public virtual bool CanUnplace() => canUnplace;

    public override void Use()
    {
        if (!canPlace || data == null)
            return;

        GameObject prefab = data.prefab;

        GameObject newObject = Instantiate(
            prefab,
            lastValidPosition,
            lastValidRotation,
            InventoryController.instance.GetItemsParent()
        );

        newObject.transform.localScale = Vector3.one;
        newObject.SetActive(true);

        if (silhouette != null)
        {
            Destroy(silhouette);
            silhouette = null;
        }

        InventoryController.instance.RemoveItem(GetItemStack());
    }

    private bool CanPlace(Vector3 position, Quaternion rotation)
    {
        return !Physics.CheckBox(
            position,
            checkBoxSize / 2f,
            rotation,
            placementMask
        );
    }

    public void ShowSilhouette(RaycastHit hit)
    {
        if (silhouette == null)
        {
            silhouette = Instantiate(data.prefab, InventoryController.instance.GetItemsParent());
            silhouette.SetActive(true);

            CalculateCheckBoxSize();
        }

        DisableSilhouetteComponents();
        AdjustSilhouette(hit);

        Renderer[] renderers = silhouette.GetComponentsInChildren<Renderer>();
        silhouette.SetActive(true);

        canPlace = CanPlace(silhouette.transform.position, silhouette.transform.rotation);

        if (canPlace)
        {
            lastValidPosition = silhouette.transform.position;
            lastValidRotation = silhouette.transform.rotation;
        }

        foreach (Renderer r in renderers)
            r.material = canPlace ? greenMaterial : redMaterial;
    }

    public void HideSilhouette()
    {
        if (silhouette != null)
        {
            canPlace = false;
            silhouette.SetActive(false);
        }
    }

    public void DeleteSilhouette()
    {
        if (silhouette != null)
            Destroy(silhouette);
    }

    private void DisableSilhouetteComponents()
    {
        foreach (var c in silhouette.GetComponentsInChildren<Collider>())
            c.enabled = false;

        foreach (var l in silhouette.GetComponentsInChildren<Light>())
            l.enabled = false;

        foreach (var p in silhouette.GetComponentsInChildren<ParticleSystem>())
            p.Stop();
    }

    private void AdjustSilhouette(RaycastHit hit)
    {
        silhouette.transform.position = hit.point;
        silhouette.transform.localScale = Vector3.one;

        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0f;

        if (camForward.sqrMagnitude < 0.001f)
            camForward = Vector3.forward;

        camForward.Normalize();

        Quaternion lookRotation = Quaternion.LookRotation(camForward);

        if (straight)
        {
            silhouette.transform.rotation =
                Quaternion.Euler(0f, lookRotation.eulerAngles.y, 0f);
        }
        else
        {
            Quaternion alignToGround =
                Quaternion.FromToRotation(Vector3.up, hit.normal);

            silhouette.transform.rotation = alignToGround * lookRotation;
        }
    }

    public void Unplace()
    {
        if (CanUnplace())
        {
            if (interact.IsPressed())
            timer += Time.deltaTime;

            if (timer > unplaceTime)
            {
                timer = 0;
                InventoryController.instance.AddItem(gameObject);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        SetCurrentHealth(Mathf.Clamp(GetCurrentHealth() - amount, 0, GetMaxHealth()));

        if (GetCurrentHealth() == 0)
            Destroy(gameObject);
    }

    public void CalculateCheckBoxSize()
    {
        if (TryGetComponent(out BoxCollider box))
            checkBoxSize = box.size;

        if (TryGetComponent(out CapsuleCollider capsule))
        {
            float r = capsule.radius;
            checkBoxSize = new Vector3(r, r, r);
        }
    }
}