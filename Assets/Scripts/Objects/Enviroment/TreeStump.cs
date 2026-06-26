using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TreeStump : HarvestableObject
{   
    [SerializeField] private List<Transform> branchSpawners;

    protected void Awake()
    {   
        if(!initialized)
            currentHealth = data.maxHealth;
    }

    public override void Harvest()
    {
        GenerateDropItems();
        Destroy(gameObject);
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

}