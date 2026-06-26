using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Radio : PlaceableBehaviour, IInteractiveObject
{   
    [SerializeField] private AudioClip interferenceSound;
    [SerializeField] private AudioClip radioSound;

    [SerializeField] private AudioClip audio1;
    [SerializeField] private AudioClip audio2;
    [SerializeField] private AudioClip audio3;
    [SerializeField] private AudioClip audio4;
    [SerializeField] private AudioClip audio5;
    [SerializeField] private AudioClip audio6;

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

        yield return new WaitForSeconds(2f);
        List<DialogueLine> conversation = new List<DialogueLine>()
        {
            new DialogueLine { speaker = "Radio", text = "This is Emergency Coordination. Do you copy? Over.", duration = 5.2f, audio = audio1 },
            new DialogueLine { speaker = "You", text = "Yes, I copy! I need evacuation now. I'm not safe here!", duration = 3f },
            new DialogueLine { speaker = "Radio", text = "Stay calm. Confirm your position.", duration = 3f, audio = audio2 },
            new DialogueLine { speaker = "You", text = "I'm at a high point, I managed to get a signal.", duration = 3f },
            new DialogueLine { speaker = "Radio", text = "Copy that. We are registering your signal.", duration = 5f, audio = audio3 },
            new DialogueLine { speaker = "Radio", text = "Nearest extraction unit is currently unavailable.", duration = 5f, audio = audio4 },
            new DialogueLine { speaker = "Radio", text = "Earliest possible evacuation window is in 14 days.", duration = 4f, audio = audio5 },
            new DialogueLine { speaker = "You", text = "Fourteen days? I might not last that long...", duration = 3f },
            new DialogueLine { speaker = "Radio", text = "Maintain position. We will monitor your signal.", duration = 4f, audio = audio6 }
        };
        DialogueController.instance.SetAudioSource(audioSource);
        DialogueController.instance.StartDialogue(conversation);
        yield return new WaitForSeconds(34f);
        canToggle = true;
        canUnplace = true;
        GameController.instance.RadioUsed(true);
        ObjectivesController.instance.NextObjective(ObjectiveType.Survive);

        yield return new WaitForSeconds(3f);
        List<DialogueLine> lines = new List<DialogueLine>()
        {
            new DialogueLine { speaker = "You", text = "Fourteen days... great.", duration = 2f }
        };

        DialogueController.instance.StartDialogue(lines);
    }
}