using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenEffectsController : MonoBehaviour
{
    [SerializeField] private Volume globalVolume;
    private PlayerAttributes player; 

    private Vignette vignetteEffect;
    private ColorAdjustments colorAdjustmentsEffect;
    private ChromaticAberration chromaticAberrationEffect;
    private LensDistortion lensDistortionEffect;

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
        if (vignetteEffect == null) return;

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
        if(colorAdjustmentsEffect == null || chromaticAberrationEffect == null || lensDistortionEffect == null) 
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
}