using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Radio : PlaceableBehaviour, IInteractiveObject
{   
    [SerializeField] private AudioClip interferenceSound;
    [SerializeField] private AudioClip radioSound;
    private bool active = false;
    private bool canToggle = true;
    private bool radioUsed = false;

    protected override void Start()
    {
        base.Start();
        active = false;
        canToggle = true;
    }

    public void Interact()
    {   
        if(canToggle)
            active = !active;

        if(active)
        {
            ObjectivesController objectives = ObjectivesController.instance;
            float requiredHeight = GameController.instance.GetRequiredHeight();
            if(transform.position.y >= requiredHeight && objectives.GetCurrentObjective() == ObjectiveType.SendSOS && !radioUsed)
            {   
                radioUsed = true;
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
                    objectives.NextObjective(ObjectiveType.HighPlace);
                }
            }
            audioSource.volume = 0.6f;
            audioSource.Play();
        }
        else
            audioSource.Stop();     
    }

    private IEnumerator RadioCR()
    {   
        canUnplace = false;
        canToggle = false;
        yield return new WaitForSeconds(20f);
        canToggle = true;
        canUnplace = true;
        GameController.instance.RadioUsed(true);
        ObjectivesController.instance.NextObjective(ObjectiveType.Survive);
    }
}