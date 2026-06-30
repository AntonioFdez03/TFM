using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class DoorBehaviour : MonoBehaviour, IInteractiveObject
{   
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private Transform rotationPivot;
    [SerializeField] private NavMeshObstacle navObstacle = null;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private float closeSounDelay = 1f;
    private GameObject pivot;
    private bool doorOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;



    protected void Start()
    {   
        if(audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if(audioSource != null)
        {
            audioSource.spatialBlend = 1;
            audioSource.minDistance = 5;
            audioSource.maxDistance = 10;
        }

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

        if(audioSource == null)
            return;
        
        if (doorOpen)
            audioSource.PlayOneShot(openSound);
        else
            StartCoroutine(SoundCR());
        
    }

    void OnDestroy()
    {   
        if (pivot != null)
            Destroy(pivot); 
    }

    private IEnumerator SoundCR()
    {
        yield return new WaitForSeconds(closeSounDelay);
        audioSource.PlayOneShot(closeSound);

    }
}