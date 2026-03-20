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

    void Start()
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
        silhouette.transform.position = hit.point;
        silhouette.transform.localScale = Vector3.one;
        silhouette.transform.rotation = Quaternion.LookRotation(hit.normal) * Quaternion.Euler(90f, 0f, 0f);
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
        print("Silueta ocultada");
        if(silhouette != null)
            silhouette.SetActive(false);
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
}