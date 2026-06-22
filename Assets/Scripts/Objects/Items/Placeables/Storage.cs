using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Storage : PlaceableBehaviour, IInteractiveObject
{
    private StorageController storageController;

    [SerializeField] protected AudioClip openSound;
    [SerializeField] protected AudioClip closeSound;

    protected override void Start()
    {
        base.Start();
        storageController = GetComponent<StorageController>();
        audioSource = GetComponent<AudioSource>();
    }

    public StorageController GetStorageController() => storageController;
    public void Interact()
    {  
        UIController.instance.OpenStorage(this);
        audioSource.PlayOneShot(openSound,0.4f);
    }

    public override bool CanUnplace() => storageController.isEmpty();
}