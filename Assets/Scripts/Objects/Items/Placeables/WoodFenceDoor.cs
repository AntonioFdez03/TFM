using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WoodFenceDoor : PlaceableBehaviour, IInteractiveObject
{
    [SerializeField] Transform rotationPivot;
    private GameObject pivot;
    private Rigidbody rb;
    private bool doorOpen = false;
    private float openAngle = 90f;
    private float openSpeed = 2f;
    private Quaternion closedRotation;
    private Quaternion openRotation;


    protected override void Start()
    {   
        base.Start();
        maxHealth = 100;
        currentHealth = maxHealth;
        pivot = new("Pivot");
        pivot.transform.rotation = transform.rotation;
        pivot.transform.SetParent(transform.parent);
        if(rotationPivot != null)
        {   
            pivot.transform.position = rotationPivot.position;
            transform.SetParent(pivot.transform);
        }
        
        closedRotation = pivot.transform.rotation;
        openRotation = Quaternion.AngleAxis(openAngle, pivot.transform.up) * closedRotation;
    }

    void Update()
    {
        Quaternion targetRotation = doorOpen ? openRotation : closedRotation;

        pivot.transform.rotation = Quaternion.Lerp(
            pivot.transform.rotation,
            targetRotation,
            Time.deltaTime * openSpeed
        );
    }

    public void Interact()
    {   
        doorOpen = !doorOpen;   
    }

    void OnDestroy()
    {   
        print("fence destroyed");
        if (pivot != null)
            Destroy(pivot); 
    }
}