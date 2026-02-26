using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TreeNode : HarvestableObject
{  
    private Rigidbody rb;
    [SerializeField] private List<Transform> logSpawners = new();
    [SerializeField] private GameObject trigger;
    
    protected override void Awake()
    {
        maxHealth = 20;
        currentHealth = maxHealth;
        toolsAccepted.Add(ToolType.Axe);
        dropItemsCount = 3;
        cooldownToDrop = 4f;
        rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }
    public override void Harvest()
    {
        rb.isKinematic = false;
        rb.useGravity = true;

        // Aplicamos torque para caer hacia adelante
        rb.AddTorque(CameraController.playerCameraInstance.transform.forward * 1500f, ForceMode.Impulse);
    }

    protected override void GenerateDropItems()
    {
        foreach (Transform spawner in logSpawners)
        {
            GameObject newItem = Instantiate(
            dropItem,
            spawner.position + Vector3.up * 0.8f,
            spawner.rotation,
            transform.parent.transform.parent
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
            Destroy(transform.parent.gameObject);
        }
    }
}