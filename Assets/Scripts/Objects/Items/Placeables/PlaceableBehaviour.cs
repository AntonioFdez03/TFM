using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceableBehaviour : ItemBehaviour
{   
    protected PlaceableData placeableData;

    protected static LayerMask placementMask;
    [SerializeField] Material greenMaterial;
    [SerializeField] Material redMaterial;
    [SerializeField] List<GameObject> brokeItemsDrop;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip brokeSound;
    protected AudioSource audioSource;

    protected Vector3 checkBoxSize;
    private GameObject silhouette;
    private Vector3 lastValidPosition;
    private Quaternion lastValidRotation;
    private Quaternion customRotation;
    private bool canPlace;
    protected bool canUnplace = true;

    private InputAction interact;
    protected float unplaceTime = 1f;
    private float timer;

    private float maxSlopeAngle = 35f;
    private HashSet<PlaceableBehaviour> touching = new();


    protected override void Start()
    {
        base.Start();
        GetComponent<Rigidbody>().isKinematic = true;
        interact = InputSystem.actions.FindAction("Interact");
        audioSource = GetComponent<AudioSource>();

        placementMask = LayerMask.GetMask(
            "Items",
            "PlacedObjects",
            "Enviroment"
        );

        CheckInitialContacts();
        placeableData = data as PlaceableData;
    }

    protected void SetCheckBoxSize(Vector3 boxSize) => checkBoxSize = boxSize; 
    public float GetCurrentTime() => timer; 
    public float SetCurrentTime(float time) => timer = time; 
    public float GetUnplaceTime() => unplaceTime;
    public virtual bool CanUnplace() => canUnplace && placeableData.canUnplace;
    public void AddTouchingItem(PlaceableBehaviour pb) => touching.Add(pb);
    public void RemoveTouchingItem(PlaceableBehaviour pb) => touching.Remove(pb);

    public override void Use()
    {
        if (!canPlace || data == null)
            return;

        AudioManager.instance.PlayOneShot("Place");

        GameObject prefab = data.prefab;

        GameObject newObject = Instantiate(
            prefab,
            lastValidPosition,
            lastValidRotation,
            InventoryController.instance.GetItemsParent()
        );

        
        newObject.GetComponent<ItemBehaviour>().SetItemStack(itemStack);

        newObject.transform.localScale = Vector3.one;
        newObject.SetActive(true);

        if (silhouette != null)
        {
            Destroy(silhouette);
            silhouette = null;
        }

        InventoryController.instance.RemoveItem(GetItemStack());
        StatisticsController.instance.AddItemPlaced();
    }

    private bool Collides(Vector3 position, Quaternion rotation)
    {
        print($"[Collide] placementMask = {placementMask.value}");
        bool hit = Physics.CheckBox(
            position,
            checkBoxSize / 2f,
            rotation,
            placementMask
        );

        print("Colliding: " + hit);
        return hit;
    }

    public void ShowSilhouette(RaycastHit hit)
    {
        if (silhouette == null)
        {
            silhouette = Instantiate(data.prefab, InventoryController.instance.GetItemsParent());
            silhouette.SetActive(true);
            customRotation = Quaternion.identity;

            CalculateCheckBoxSize();

            if (data is PlaceableData placeableData)
                print(placeableData.straight);

            DisableSilhouetteComponents(); 
        }
        AdjustSilhouette(hit);

        silhouette.SetActive(true);
        silhouette.GetComponent<Rigidbody>().isKinematic = true;

        canPlace = IsSlopeValid(hit) && !Collides(silhouette.transform.position, silhouette.transform.rotation);

        if (canPlace)
        {
            lastValidPosition = silhouette.transform.position;
            lastValidRotation = silhouette.transform.rotation;
        }

        Material targetMat = canPlace ? greenMaterial : redMaterial;
        Renderer[] renderers = silhouette.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            Material[] mats = r.materials;

            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = targetMat;
            }

            r.materials = mats;
        }
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

        foreach (var chest in silhouette.GetComponentsInChildren<WoodChest>())
        {
            chest.enabled = false;
        }

        DoorBehaviour door = silhouette.GetComponentInChildren<DoorBehaviour>();

        if (door != null)
        {
            door.enabled = false;
        }
    }

    private void AdjustSilhouette(RaycastHit hit)
    {
        silhouette.transform.position = hit.point;
        silhouette.transform.localScale = Vector3.one;

        Vector3 silhoueteForward = Camera.main.transform.position - hit.point;
        silhoueteForward.y = 0f;

        silhoueteForward.Normalize();

        Quaternion lookRotation = Quaternion.LookRotation(silhoueteForward);
        bool straight = false;

        if (data is PlaceableData placeableData)
            straight = placeableData.straight;

        if (straight)
        {   
            silhouette.transform.rotation = Quaternion.Euler(0f, lookRotation.eulerAngles.y, 0f) * customRotation;
        }
        else
        {
            Quaternion alignToGround =
                Quaternion.FromToRotation(Vector3.up, hit.normal);

            silhouette.transform.rotation = alignToGround * lookRotation * customRotation;
        }
            
    }

    public void RotateSilhouette()
    {   
        customRotation *= Quaternion.Euler(0, 70 * Time.deltaTime, 0);
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
        if(audioSource != null)
            audioSource.PlayOneShot(hitSound);

        SetCurrentHealth(Mathf.Clamp(GetCurrentHealth() - amount, 0, GetMaxHealth()));

        if (GetCurrentHealth() == 0)
        {   
            Vector3 center = GetComponentInChildren<Renderer>().bounds.center;
            for(int i = 0; i < brokeItemsDrop.Count; i++)
            {
                GameObject dropItem = Instantiate(brokeItemsDrop[i], InventoryController.instance.GetItemsParent());
                Vector3 offset = new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(0.1f, 0.5f), UnityEngine.Random.Range(-0.3f, 0.3f));
                dropItem.transform.position = center + offset; 
            }
            AudioManager.instance.PlayOneShot("WoodBroken");
            ManagePlacedItems();
            Destroy(gameObject);
        }
            
    }

    public void Heal(float amount)
    {
        SetCurrentHealth(Mathf.Clamp(GetCurrentHealth() + amount, 0, GetMaxHealth()));
    }

    private void CalculateCheckBoxSize()
    {
        if (silhouette == null)
            return;

        Collider col = silhouette.GetComponentInChildren<Collider>();

        if (col == null)
        {
            Debug.LogWarning("No collider found in silhouette");
            return;
        }

        switch (col)
        {
            case BoxCollider box:
                checkBoxSize = Vector3.Scale(
                    box.size,
                    box.transform.lossyScale
                );
                break;

            case CapsuleCollider capsule:
                float radius = capsule.radius;

                Vector3 scale = capsule.transform.lossyScale;

                // Escala horizontal promedio para el radio
                float scaledRadius =
                    radius * Mathf.Max(scale.x, scale.z);

                float diameter = scaledRadius * 2f;

                checkBoxSize = new Vector3(
                    diameter,
                    capsule.height * scale.y,
                    diameter
                );
                break;

            case MeshCollider mesh:
                checkBoxSize = Vector3.Scale(
                    mesh.sharedMesh.bounds.size,
                    mesh.transform.lossyScale
                );
                break;

            default:
                // Fallback para cualquier collider raro
                checkBoxSize = col.bounds.size;
                break;
        }

        Debug.Log($"CheckBox Size: {checkBoxSize}");
    }

    private bool IsSlopeValid(RaycastHit hit)
    {
        float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);
        return slopeAngle <= maxSlopeAngle;
    }


    public void ManagePlacedItems()
    {
        foreach (var placeable in touching)
        {
            if (placeable == null || placeable == this)
                continue;

            if (placeable is Radio)
            {
                Rigidbody rb = placeable.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                }
            }
            else
            {
                Destroy(placeable.gameObject);
            }
        }

        touching.Clear();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out PlaceableBehaviour pb))
        {   
            print("Item añadido: " + pb.gameObject.name);
            touching.Add(pb);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out PlaceableBehaviour pb))
        {
            print("Item retirado: " + pb.gameObject.name);
            touching.Remove(pb);
        }
    }

    private void CheckInitialContacts()
    {
        Collider col = GetComponentInChildren<Collider>();
        if (col == null) return;

        Bounds bounds = col.bounds;

        Collider[] hits = Physics.OverlapBox(
            bounds.center,
            bounds.extents,
            transform.rotation
        );

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            if (hit.TryGetComponent(out PlaceableBehaviour pb))
            {
                touching.Add(pb);
                print("Inicial añadido: " + pb.name);
            }
        }
    }
}