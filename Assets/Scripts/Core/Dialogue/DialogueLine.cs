using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
    public float duration = 2f;
    public AudioClip audio;
}