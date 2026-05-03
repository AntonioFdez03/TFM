using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenEffectsController : MonoBehaviour
{
    [SerializeField] private Volume globalVolume;
    private PlayerAttributes player; 

    private Vignette vignetteEffect;

    void Start()
    {
        player = PlayerController.instance.GetPlayerAttributes();

        player.OnHealthChanged += HandleHealthEffect;
        player.OnSanityChanged += HandleSanityEffect;

        if (!globalVolume.profile.TryGet<Vignette>(out vignetteEffect))
            print("No se encontró Vignette en el Volume ACTIVO");
        
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
        // Aquí puedes meter blur, distorsión, etc.
    }
}