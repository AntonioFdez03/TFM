using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BigTreeNode : HarvestableObject
{  
    private Rigidbody rb;
    [SerializeField] private List<Transform> logSpawners = new();
    [SerializeField] private GameObject trigger;
    
    protected override void Awake()
    {
        base.Awake();
        objectName = "BigTree";
        maxHealth = 100;
        currentHealth = maxHealth;
        toolsAccepted.Add(ToolType.Axe);
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
        rb.AddForce(fallDirection * 1000f, ForceMode.Impulse);
    }

    protected void GenerateDropItems()
    {
        foreach (Transform spawner in logSpawners)
        {
            GameObject newItem = Instantiate(
            dropItem,
            spawner.position + Vector3.up * 0.8f,
            spawner.rotation,
            InventoryController.instance.GetItemsParent()
            );

            Rigidbody itemRB = newItem.GetComponent<Rigidbody>();

            itemRB.linearVelocity = Vector3.zero;
            itemRB.angularVelocity = Vector3.zero;
            itemRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        print(other);
        if (other.CompareTag("Terrain"))
        {
            GenerateDropItems();
            Destroy(gameObject);
        }
    }
}