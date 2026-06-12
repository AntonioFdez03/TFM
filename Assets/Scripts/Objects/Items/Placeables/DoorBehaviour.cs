using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DoorBehaviour : MonoBehaviour, IInteractiveObject
{
    [SerializeField] Transform rotationPivot;
    [SerializeField] private NavMeshObstacle navObstacle = null;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;
    private GameObject pivot;
    private bool doorOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;


    protected void Start()
    {   
        closedRotation = rotationPivot.rotation;
        openRotation = Quaternion.AngleAxis(openAngle, rotationPivot.up) * closedRotation;

        if(navObstacle != null)
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