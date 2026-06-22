using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Rock : HarvestableObject
{   

    [SerializeField] private int rockPhase = 0;
    [SerializeField] private List<Mesh> rockMeshes;
    [SerializeField] private List<Transform> dropPositions;
    private Rigidbody rb;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    protected void Awake()
    {   
        currentHealth = data.maxHealth;
        rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = rockMeshes[rockPhase];

        meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = rockMeshes[rockPhase];
    }

    public override void Harvest()
    {   
        rockPhase += 1;
        if(rockPhase < rockMeshes.Count)
        {   
            currentHealth = data.maxHealth;
            meshFilter.mesh = rockMeshes[rockPhase];
            meshCollider.sharedMesh = rockMeshes[rockPhase];

            Collider[] hits = Physics.OverlapSphere(dropPositions[1].position, 5f);

            foreach (Collider hit in hits)
            {
                print("Colisiona: " + hit.name);
                Rigidbody rb = hit.attachedRigidbody;

                if (rb != null)
                    rb.WakeUp();
            }

            DropItem();
            DropItem();
        }
        else
        {
            DropItem();
            DropItem();
            Destroy(gameObject);
        }
    }

    private void DropItem()
    {
        GameObject dropItemInstance = Instantiate(dropItem, dropPositions[rockPhase-1].position, Quaternion.identity);

        dropItemInstance.transform.SetParent(InventoryController.instance.GetItemsParent());

        Rigidbody dropItemRB = dropItemInstance.GetComponent<Rigidbody>();
        if(dropItemRB != null)
            dropItemRB.isKinematic = false;
    }
}