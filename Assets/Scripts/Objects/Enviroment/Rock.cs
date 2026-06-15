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
    private Rigidbody rb;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    protected void Awake()
    {   
        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
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
        DropItem();
        DropItem();
        rockPhase += 1;
        if(rockPhase < rockMeshes.Count)
        {   
            currentHealth = maxHealth;
            meshFilter.mesh = rockMeshes[rockPhase];
            //meshCollider.sharedMesh = rockMeshes[rockPhase];
        }else
            Destroy(gameObject);
    }

    private void DropItem()
    {
        Vector3 dropPosition = transform.position + Vector3.up * 4;

        GameObject dropItemInstance = Instantiate(dropItem, dropPosition, Quaternion.identity);

        dropItemInstance.transform.SetParent(InventoryController.instance.GetItemsParent());

        Rigidbody dropItemRB = dropItemInstance.GetComponent<Rigidbody>();
        if(dropItemRB != null)
        {
            // Genera una dirección aleatoria horizontal
            Vector3 randomDirection = new Vector3(Random.Range(-0.5f, 0.5f), 0.5f, Random.Range(-0.5f, 0.5f)).normalized;

            float forceMagnitude = Random.Range(50f, 100f);
            dropItemRB.isKinematic = false;
            dropItemRB.AddForce(randomDirection * forceMagnitude, ForceMode.Impulse);
        }
    }
}