using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{   
    [SerializeField] private Transform sanityBar;
    [SerializeField] private Transform staminaBar;
    [SerializeField] private Transform healthBar;
    [SerializeField] private Transform hungerBar;

    private PlayerAttributes player;

    private float intensity = 0.1f;
    private float defaultBeatSpeed = 5f;
    private float sanityBeatSpeed = 5f;
    private float healthBeatSpeed = 5f;
    private float staminaBeatSpeed = 5f;
    private float hungerBeatSpeed = 5f;

    private Vector3 sanityOriginalScale;
    private Vector3 staminaOriginalScale;
    private Vector3 healthOriginalScale;
    private Vector3 hungerOriginalScale;

    void Start()
    {   
        player = PlayerController.instance.GetPlayerAttributes();

        sanityOriginalScale = sanityBar.transform.localScale;
        staminaOriginalScale = staminaBar.transform.localScale;
        healthOriginalScale = healthBar.transform.localScale;
        hungerOriginalScale = hungerBar.transform.localScale;
    }

    void Update()
    {   
        UpdateSpeedValues();

        sanityBar.transform.localScale = sanityOriginalScale;
        staminaBar.transform.localScale = staminaOriginalScale;
        healthBar.transform.localScale = healthOriginalScale;
        hungerBar.transform.localScale = hungerOriginalScale;

        Beat(sanityBar, sanityOriginalScale, sanityBeatSpeed);
        Beat(staminaBar, staminaOriginalScale, staminaBeatSpeed);
        Beat(healthBar, healthOriginalScale, healthBeatSpeed);
        Beat(hungerBar, hungerOriginalScale, hungerBeatSpeed);

        UpdateStatsBar();
    }

    private void Beat(Transform stat, Vector3 originalScale, float speed)
    {
        float pulse = Mathf.Sin(Time.time * speed) * intensity;
        stat.localScale = originalScale + originalScale * pulse;
    }

    private void UpdateSpeedValues()
    {   
        float healthPercent = player.GetCurrentHealth() / player.GetMaxHealth();

        if(healthPercent < 0.15f)
            healthBeatSpeed = defaultBeatSpeed * 3;

        else if(healthPercent < 0.3f)
            healthBeatSpeed = defaultBeatSpeed * 2;

        else if(healthPercent < 0.5f)
            healthBeatSpeed = defaultBeatSpeed;

        else 
            healthBeatSpeed = 0f;

        float sanityPercent = player.GetCurrentSanity() / player.GetMaxSanity();

        if(sanityPercent < 0.15f)
            sanityBeatSpeed = defaultBeatSpeed * 3;

        else if(sanityPercent < 0.3f)
            sanityBeatSpeed = defaultBeatSpeed * 2;

        else if(sanityPercent < 0.5f)
            sanityBeatSpeed = defaultBeatSpeed;

        else 
            sanityBeatSpeed = 0f;

        float staminaPercent = player.GetCurrentStamina() / player.GetMaxStamina();

        if(staminaPercent == 0f)
            staminaBeatSpeed = defaultBeatSpeed;
        else 
            staminaBeatSpeed = 0f;

        float hungerPercent = player.GetCurrentHunger() / player.GetMaxHunger();

        if(hungerPercent < 0.15f)
            hungerBeatSpeed = defaultBeatSpeed * 3;

        else if(hungerPercent < 0.3f)
            hungerBeatSpeed = defaultBeatSpeed * 2;

        else if(hungerPercent < 0.5f)
            hungerBeatSpeed = defaultBeatSpeed;
        
        else 
            hungerBeatSpeed = 0f;
    }

    

    private void UpdateStatsBar()
    {   
        if(sanityBar != null)
            sanityBar.GetChild(0).GetComponent<Image>().fillAmount = player.GetCurrentSanity()/player.GetMaxSanity();
        if(staminaBar != null)
            staminaBar.GetChild(0).GetComponent<Image>().fillAmount = player.GetCurrentStamina()/player.GetMaxStamina();
        if(healthBar != null)
            healthBar.GetChild(0).GetComponent<Image>().fillAmount = player.GetCurrentHealth()/player.GetMaxHealth();
        if(hungerBar != null)
            hungerBar.GetChild(0).GetComponent<Image>().fillAmount = player.GetCurrentHunger()/player.GetMaxHunger();
    }
}