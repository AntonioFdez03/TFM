using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WoodChest : Storage
{   
    [SerializeField] private Transform rotationPivot;
    private float openAngle = -60f;
    private float speed = 3f;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool chestOpen = false;

    protected override void Start()
    {   
        base.Start();
        closedRotation = rotationPivot.rotation;
        openRotation = Quaternion.AngleAxis(openAngle, rotationPivot.right) * closedRotation;
    }

    void Update()
    {   
        chestOpen = UIController.instance.GetCurrentState() == UIState.Storage;
        Quaternion targetRotation = chestOpen ? openRotation : closedRotation;

        rotationPivot.rotation = Quaternion.Lerp(
            rotationPivot.rotation,
            targetRotation,
            Time.deltaTime * speed
        );
    }
}