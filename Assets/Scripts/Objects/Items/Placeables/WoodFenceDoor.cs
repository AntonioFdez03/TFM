using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class WoodFenceDoor : PlaceableBehaviour, IInteractiveObject
{
    [SerializeField] Transform rotationPivot;
    [SerializeField] private NavMeshObstacle navObstacle;
    private GameObject pivot;
    private bool doorOpen = false;
    private float openAngle = 90f;
    private float openSpeed = 2f;
    private Quaternion closedRotation;
    private Quaternion openRotation;


    protected override void Start()
    {   
        base.Start();
        
        closedRotation = rotationPivot.rotation;
        openRotation = Quaternion.AngleAxis(openAngle, rotationPivot.up) * closedRotation;

        navObstacle.carveOnlyStationary = false;
    }

    void Update()
    {
        Quaternion targetRotation = doorOpen ? openRotation : closedRotation;

        rotationPivot.rotation = Quaternion.Lerp(
            rotationPivot.rotation,
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
        if (pivot != null)
            Destroy(pivot); 
    }
}