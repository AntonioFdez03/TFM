using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DialogueController : MonoBehaviour
{
    public static DialogueController instance;

    private AudioSource audioSource;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;

    private Queue<DialogueLine> queue = new Queue<DialogueLine>();
    private bool playing = false;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        HideDialogue();
        
    }

    public void SetAudioSource(AudioSource source) => audioSource = source;
    public void StartDialogue(List<DialogueLine> lines)
    {
        queue.Clear();

        foreach (var line in lines)
            queue.Enqueue(line);

        if (!playing)
            StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        playing = true;

        while (queue.Count > 0)
        {
            var line = queue.Dequeue();

            speakerText.text = line.speaker;
            dialogueText.text = line.text;

            if (line.audio != null && audioSource != null)
            {   
                audioSource.volume = 1f;
                audioSource.PlayOneShot(line.audio);
            }

            yield return new WaitForSeconds(line.duration);
        }

        HideDialogue();
        playing = false;
    }

    private void HideDialogue()
    {
        speakerText.text = "";
        dialogueText.text = "";
    }
}