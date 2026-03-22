using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlaceableBehaviour : ItemBehaviour
{
    [SerializeField] Material greenMaterial;
    [SerializeField] Material redMaterial;
    [SerializeField] LayerMask placementMask;
    private Vector3 checkBoxSize = new Vector3(3f,3f,3f);
    private GameObject silhouette;
    private Vector3 lastValidPosition;
    private Quaternion lastValidRotation;
    private bool canPlace;
    protected bool straight = false;

    protected virtual void Start()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    public override void Use()
    {   
        if(!canPlace || itemData == null)
            return;

        GameObject newObject = Instantiate( itemData.GetItemPrefab(),lastValidPosition, lastValidRotation, InventoryController.instance.GetItemsParent());
        newObject.transform.localScale = Vector3.one;
        newObject.SetActive(true);
        newObject.GetComponent<Rigidbody>().isKinematic = true;

        if (silhouette != null)
        {
            Destroy(silhouette);
            silhouette = null;
        }
        
        InventoryController.instance.RemoveItem(HotBarController.instance.GetCurrentItem());
    }

    public void ShowSilhouette(RaycastHit hit)
    {   
        if(silhouette == null)
        {   
            itemData = GetComponent<ItemData>();
            silhouette = Instantiate(itemData.GetItemPrefab(), InventoryController.instance.GetItemsParent());
            silhouette.SetActive(true);
        }
        DisableSilhouetteComponents();
        AdjustSilhouette(hit);
        Renderer[] renderers = silhouette.GetComponentsInChildren<Renderer>();
        silhouette.SetActive(true);

        canPlace = CanPlace(silhouette.transform.position, silhouette.transform.rotation);

        if(canPlace)
        {
            lastValidPosition = silhouette.transform.position;
            lastValidRotation = silhouette.transform.rotation;
        }

        foreach (Renderer r in renderers)
            r.material = canPlace ? greenMaterial : redMaterial;
    }

    public void HideSilhouette()
    {   
        if(silhouette != null)
        {
            canPlace = false;
            silhouette.SetActive(false);
        }
    }

    public void DeleteSilhouette()
    {
        if(silhouette != null)
            Destroy(silhouette);
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

    private void DisableSilhouetteComponents()
    {
        Collider[] colliders = silhouette.GetComponentsInChildren<Collider>();
        Light[] lights = silhouette.GetComponentsInChildren<Light>();
        ParticleSystem[] particles = silhouette.GetComponentsInChildren<ParticleSystem>();

        foreach (Collider c in colliders)
            c.enabled = false;

        foreach (Light l in lights)
            l.enabled = false;
            
        foreach (ParticleSystem p in particles)
            p.Stop();
    }

    private void AdjustSilhouette(RaycastHit hit)
    {   
        silhouette.transform.position = hit.point;
        silhouette.transform.localScale = Vector3.one;

        // Dirección de la cámara en plano horizontal
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0f;

        // Evitar vector cero
        if (camForward.sqrMagnitude < 0.001f)
            camForward = Vector3.forward;

        camForward.Normalize();

        Quaternion lookRotation = Quaternion.LookRotation(camForward);

        if (straight)
        {
            print("Recto");
            silhouette.transform.rotation = Quaternion.Euler(0f, lookRotation.eulerAngles.y, 0f);
        }
        else
        {
            print("Torcido");
            Quaternion alignToGround = Quaternion.FromToRotation(Vector3.up, hit.normal);
            silhouette.transform.rotation = alignToGround * lookRotation;
        }
    }
}