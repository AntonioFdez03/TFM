using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BigTree : HarvestableObject
{  
    [SerializeField] private GameObject cutTreePrefab;
    [SerializeField] private GameObject stumpPrefab;
    private Rigidbody rb;
    
    protected void Awake()
    {
        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
        toolsAccepted.Add(ToolType.Axe);
        rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public override void Harvest()
    {
        GameObject stump = Instantiate(stumpPrefab, InventoryController.instance.GetItemsParent());
        stump.transform.position = transform.position;

        GameObject cutTree = Instantiate(cutTreePrefab, InventoryController.instance.GetItemsParent());
        cutTree.transform.position = new Vector3(transform.position.x, transform.position.y + 5f, transform.position.z);

        Vector3 fallDirection = PlayerController.instance.transform.forward;
        Rigidbody cutTreeRB =  cutTree.GetComponent<Rigidbody>();
        cutTreeRB.isKinematic = false;
        cutTreeRB.useGravity = true;
        cutTreeRB.AddForce(fallDirection * 200f, ForceMode.Impulse);

        Destroy(gameObject);
    }
}