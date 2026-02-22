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

        rb.AddTorque(transform.right * Random.Range(30f, 50f), ForceMode.Impulse);
    }

    protected override void GenerateDropItems()
    {
        foreach (Transform spawner in logSpawners)
        {
            // Instanciamos el tronco en la posición y rotación del Empty
            GameObject log = Instantiate(dropItem, spawner.position, spawner.rotation, transform.parent);
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