using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WoodChest : Storage
{   
    [SerializeField] private Transform rotationPivot;
    private float openAngle = -60f;
    private float speed;
    private float openSpeed = 3f;
    private float closeSpeed = 9f;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool chestOpen = false;
    private bool wasFullyClosed = true;

    protected override void Start()
    {   
        base.Start();
        closedRotation = rotationPivot.rotation;
        openRotation = Quaternion.AngleAxis(openAngle, rotationPivot.right) * closedRotation;
    }

    void Update()
    {
        chestOpen = UIController.instance.GetCurrentState() == UIState.Storage &&
                    UIController.instance.GetCurrentStorage() == this;

        Quaternion targetRotation = chestOpen ? openRotation : closedRotation;
        speed = chestOpen ? openSpeed : closeSpeed;

        rotationPivot.rotation = Quaternion.Lerp(
            rotationPivot.rotation,
            targetRotation,
            Time.deltaTime * speed
        );

        bool isFullyClosed =
            !chestOpen &&
            Quaternion.Angle(rotationPivot.rotation, closedRotation) < 0.5f;

        if (!wasFullyClosed && isFullyClosed)
        {
            audioSource.PlayOneShot(closeSound, 0.2f);
        }

        wasFullyClosed = isFullyClosed;
    }

}