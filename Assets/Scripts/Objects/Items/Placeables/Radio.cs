using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Radio : PlaceableBehaviour, IInteractiveObject
{   
    [SerializeField] private AudioClip interferenceSound;
    [SerializeField] private AudioClip radioSound;
    private AudioSource audioSource;
    private bool active = false;
    private bool canToggle = true;
    private float requiredHeight = 250;

    protected override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact()
    {   
        if(canToggle)
            active = !active;

        if(active)
        {
            ObjectivesController objectives = ObjectivesController.instance;
            if(transform.position.y >= requiredHeight)
            {
                audioSource.clip = radioSound;
                audioSource.loop = false;
                StartCoroutine(RadioCR());
            }
            else
            {
                audioSource.clip = interferenceSound;
                audioSource.loop = true;
                if (objectives.GetCurrentObjective() == ObjectiveType.Explore)
                {
                    objectives.NextObjective(ObjectiveType.RadioSignal);
                }
            }
            audioSource.volume = 4f;
            audioSource.Play();
        }
        else
            audioSource.Stop();     
    }

    private IEnumerator RadioCR()
    {
        canToggle = false;
        yield return new WaitForSeconds(20f);
        canToggle = true;
        ObjectivesController.instance.NextObjective(ObjectiveType.Survive);
    }
}