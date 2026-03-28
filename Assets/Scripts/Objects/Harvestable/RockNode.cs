using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class RockNode : HarvestableObject
{  
    [SerializeField] private Transform dropPosition;
    private Rigidbody rb;
    private bool firstDrop = false;
    private bool secondDrop = false;
    private bool thirdDrop = false;
    private bool fourthDrop = false;

    protected override void Awake()
    {   
        base.Awake();
        objectName = "Rock";
        maxHealth = 100;
        currentHealth = maxHealth;
        toolsAccepted.Add(ToolType.Pickaxe);
        rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public override void TakeHit(ToolType tool, float damage)
    {
        base.TakeHit(tool,damage);
        if(currentHealth < 0.75f * maxHealth && !firstDrop)
            GenerateDropItem(ref firstDrop);

        if(currentHealth < 0.5f * maxHealth && !secondDrop)
            GenerateDropItem(ref secondDrop);

        if(currentHealth < 0.25f * maxHealth && !thirdDrop)
            GenerateDropItem(ref thirdDrop);
    }

    public override void Harvest()
    {   
        GenerateDropItem(ref fourthDrop);
        Destroy(gameObject);
    }

    protected void GenerateDropItem(ref bool drop)
    {  
        drop = true;
        GameObject dropItemInstance = Instantiate(dropItem,dropPosition.position, Quaternion.identity);
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