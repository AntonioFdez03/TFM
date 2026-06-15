using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenEffectsController : MonoBehaviour
{   
    public static ScreenEffectsController instance;
    [SerializeField] private Volume globalVolume;
    private PlayerAttributes player; 

    private Vignette vignetteEffect;
    private ColorAdjustments colorAdjustmentsEffect;
    private ChromaticAberration chromaticAberrationEffect;
    private LensDistortion lensDistortionEffect;

    private bool isDying = false;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        player = PlayerController.instance.GetPlayerAttributes();

        player.OnHealthChanged += HandleHealthEffect;
        player.OnSanityChanged += HandleSanityEffect;

        if (!globalVolume.profile.TryGet(out vignetteEffect))
            print("No se encontró Vignette en el Volume ACTIVO");
        else if(!globalVolume.profile.TryGet(out colorAdjustmentsEffect))
            print("No se encontró ColorAdjustements en el Volume ACTIVO");
        else if(!globalVolume.profile.TryGet(out chromaticAberrationEffect))
            print("No se encontró ChromaticAberration en el Volume ACTIVO");
        else if(!globalVolume.profile.TryGet(out lensDistortionEffect))
            print("No se encontró Lens Distorsion en el Volume ACTIVO");
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnHealthChanged -= HandleHealthEffect;
            player.OnSanityChanged -= HandleSanityEffect;
        }
    }

    public void HandleHealthEffect(float healthPercent)
    {
        if (vignetteEffect == null || isDying) return;

        if (healthPercent < 0.9f)
        {
            vignetteEffect.color.value = Color.red;
            vignetteEffect.intensity.value = 0.4f;
            vignetteEffect.smoothness.value = 0.3f;
        }
        else
        {
            vignetteEffect.color.value = Color.black;
            vignetteEffect.intensity.value = 0.3f;
            vignetteEffect.smoothness.value = 0.2f;
        }
    }

    public void HandleSanityEffect(float sanityPercent)
    {
        if(colorAdjustmentsEffect == null || chromaticAberrationEffect == null || lensDistortionEffect == null || isDying) 
            return;

        if(sanityPercent < 0.9f)
        {
            colorAdjustmentsEffect.active = true;
            colorAdjustmentsEffect.postExposure.value = -1;
            colorAdjustmentsEffect.contrast.value = -10;
            colorAdjustmentsEffect.colorFilter.value = new Color(150f/255f, 150f/255f, 150f/255f);
            colorAdjustmentsEffect.hueShift.value = -25;
            colorAdjustmentsEffect.saturation.value = -70;

            chromaticAberrationEffect.active = true;
            chromaticAberrationEffect.intensity.value = 1;

            lensDistortionEffect.active = true;
            lensDistortionEffect.intensity.value = 0.4f;
            lensDistortionEffect.xMultiplier.value = 1;
            lensDistortionEffect.yMultiplier.value = 1;
            lensDistortionEffect.center.value = new Vector2(0.5f,0.5f);
            lensDistortionEffect.scale.value = 1;
        }
        else
        {
            colorAdjustmentsEffect.active = false;
            chromaticAberrationEffect.active = false;
            lensDistortionEffect.active = false;
        }
    }

    public void PlayDeathEffect(float duration = 2f)
    {
        if (isDying) return;
        StartCoroutine(DeathEffectCR(duration));
    }

    private IEnumerator DeathEffectCR(float duration)
    {
        isDying = true;

        float timer = 0f;

        float startVignette = vignetteEffect.intensity.value;
        float startSmoothness = vignetteEffect.smoothness.value;

        vignetteEffect.active = true;

        // IMPORTANTE: reset limpio
        vignetteEffect.color.value = Color.black;

        float phase1 = duration * 0.4f; // inicio lento
        float phase2 = duration * 0.6f; // cierre fuerte

        // =========================
        // FASE 1: empieza a “cerrar visión”
        // =========================
        while (timer < phase1)
        {
            timer += Time.deltaTime;
            float t = timer / phase1;

            float smoothT = t * t;

            vignetteEffect.intensity.value =
                Mathf.Lerp(startVignette, 0.6f, smoothT);

            vignetteEffect.smoothness.value =
                Mathf.Lerp(startSmoothness, 0.4f, smoothT);

            yield return null;
        }

        timer = 0f;

        // =========================
        // FASE 2: cierre de ojos real
        // =========================
        while (timer < phase2)
        {
            timer += Time.deltaTime;
            float t = timer / phase2;

            float smoothT = Mathf.Pow(t, 2.5f);

            vignetteEffect.intensity.value =
                Mathf.Lerp(0.6f, 1f, smoothT);

            vignetteEffect.smoothness.value =
                Mathf.Lerp(0.4f, 1f, smoothT); // 🔥 esto simula el “párpado cerrándose”

            yield return null;
        }

        // blackout total
        vignetteEffect.intensity.value = 1f;
        vignetteEffect.smoothness.value = 1f;

        yield return new WaitForSeconds(0.2f);

        GameController.instance.GameOver();
    }
}