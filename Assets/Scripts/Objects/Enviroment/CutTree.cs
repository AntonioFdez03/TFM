using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CutTree : HarvestableObject
{   
    [SerializeField] private List<Transform> logSpawners = new();
    [SerializeField] private GameObject trigger;
    
    protected override void Awake()
    {
        base.Awake();
        objectName = "CutTree";
        maxHealth = 50;
        currentHealth = maxHealth;
        toolsAccepted.Add(ToolType.Axe);  
    }

    public override void Harvest()
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

        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Terrain"))
        {
            //Sonido
        }
    }
}