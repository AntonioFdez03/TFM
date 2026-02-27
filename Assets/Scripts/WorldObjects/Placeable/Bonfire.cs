using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bonfire : PlaceableObject
{
    private bool burnPlayer;
    private float fireDamage;

    void Start()
    {
        fireDamage = 20;
    }

    void Update()
    {
        if (burnPlayer)
        {
            PlayerController.playerInstance.GetPlayerAttributes().TakeDamage(fireDamage);
        }
    }

    public override void Place()
    {
        
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