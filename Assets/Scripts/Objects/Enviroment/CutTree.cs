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
    private Rigidbody rb;

    private bool onTerrain = false;
    private float treeDamage = 20;
    
    protected void Awake()
    {   
        if(!initialized)
            currentHealth = data.maxHealth;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
            onTerrain = true;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(!onTerrain && rb.linearVelocity.sqrMagnitude > 1f && collision.gameObject.CompareTag("Player"))
        {   
            Vector3 pushDir = (transform.position - collision.transform.position).normalized;
            pushDir.y = 0f; // evitar que lo levante

            rb.AddForce(pushDir * 8, ForceMode.Impulse);
            PlayerController.instance.GetPlayerAttributes().TakeDamage(treeDamage);
        }
    }
}