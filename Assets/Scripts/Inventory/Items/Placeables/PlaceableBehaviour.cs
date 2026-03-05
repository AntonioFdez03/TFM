using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlaceableBehaviour : ItemBehaviour
{
    [SerializeField] Transform itemsLayer;
    private RaycastHit hit;

    public void SetRayHit(RaycastHit h) => hit = h;
    public override void Use()
    {
        GameObject obj = Instantiate(itemData.GetItemPrefab(), itemsLayer, false);

        // Ajustar altura automáticamente
        float yOffset = 0f;

        if (obj.TryGetComponent(out Collider col))
            yOffset = col.bounds.extents.y;

        obj.transform.position = hit.point + Vector3.up * yOffset;

        // Opcional: alinear a la inclinación
        obj.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

        // Remover del inventario
        InventoryController.instance.RemoveItem(HotBarController.instance.GetCurrentItem());
    }
}