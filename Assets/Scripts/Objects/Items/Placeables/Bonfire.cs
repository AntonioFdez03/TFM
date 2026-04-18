using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bonfire : PlaceableBehaviour
{
    private bool burnPlayer;
    private float fireDamage;

    protected override void Start()
    {   
        base.Start();
        fireDamage = 20;
    }

    void Update()
    {
        if (burnPlayer)
        {
            PlayerController.instance.GetPlayerAttributes().TakeDamage(fireDamage);
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            burnPlayer = true;
        
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            burnPlayer = false;
    }
}