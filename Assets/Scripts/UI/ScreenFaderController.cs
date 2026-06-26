using UnityEngine;
using System.Collections;

public class ScreenFaderController : MonoBehaviour
{  
    [SerializeField] private CanvasGroup fadeImage;
    private float fadeDuration;
    
    public void FadeIn()
    {   
        fadeDuration = 3f;
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(Fade(1f, 0f));
    }

    public void FadeOut()
    {   
        fadeDuration = 1f;
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(Fade(0f, 1f));
    }

    IEnumerator Fade(float start, float end)
    {       
        fadeImage.alpha = start;
        fadeImage.blocksRaycasts = true;
        float time = 0;

        while (time < fadeDuration)
        {   
            print("Fade");
            time += Time.unscaledDeltaTime;
            fadeImage.alpha = Mathf.Lerp(start, end, time / fadeDuration);
            yield return null;
        }

        fadeImage.alpha = end;
        fadeImage.blocksRaycasts = false;
    }
}