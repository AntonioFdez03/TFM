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
        print("Objeto colocado");
    }
}