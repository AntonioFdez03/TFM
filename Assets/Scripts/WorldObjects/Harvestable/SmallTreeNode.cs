using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SmallTreeNode : HarvestableObject
{  
    private Rigidbody rb;
    [SerializeField] private List<Transform> branchSpawners;

    protected override void Awake()
    {
        base.Awake();
        maxHealth = 40;
        currentHealth = maxHealth;
        toolsAccepted.Add(ToolType.Axe);
        toolsAccepted.Add(ToolType.None);
        rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public override void Harvest()
    {
        rb.isKinematic = false;
        rb.useGravity = true;

        transform.position += Vector3.up * 0.5f;

        Vector3 fallDirection = PlayerController.instance.transform.forward;
        rb.AddForce(fallDirection * 80f, ForceMode.Impulse);
    }

    protected void GenerateDropItems()
    {
        foreach (Transform spawner in branchSpawners)
        {
            GameObject newBranch = Instantiate(
            dropItem,
            spawner.position + Vector3.up * 0.8f,
            spawner.rotation,
            InventoryController.instance.GetItemsParent()
            );

            Rigidbody itemRB = newBranch.GetComponent<Rigidbody>();

            itemRB.linearVelocity = Vector3.zero;
            itemRB.angularVelocity = Vector3.zero;
            itemRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Terrain"))
        {
            GenerateDropItems();
            Destroy(gameObject);
        }
    }
}