using UnityEngine;
using System.Collections;

public class ScreenFaderController : MonoBehaviour
{   
    public static ScreenFaderController instance;

    [SerializeField] private CanvasGroup fadePanel;
    private float fadeDuration = 1f;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
    public void FadeIn()
    {
        fadePanel.gameObject.SetActive(true);
        StartCoroutine(Fade(1f, 0f));
    }

    public void FadeOut()
    {   
        print("Fade out");
        fadePanel.gameObject.SetActive(true);
        StartCoroutine(Fade(0f, 1f));
    }

    IEnumerator Fade(float start, float end)
    {   
        fadePanel.alpha = start;

        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(start, end, time / fadeDuration);
            yield return null;
        }

        fadePanel.alpha = end;
    }
}