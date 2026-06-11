using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameplayUI : MonoBehaviour
{   
    public static GameplayUI instance;

    // Stats del jugador
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

    // Info del item
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemHealth;
    [SerializeField] Image itemHealthSlider;
    [SerializeField] Slider circularSlider;

    //Teclas
    [SerializeField] Transform keysLayout;
    [SerializeField] GameObject keyInfoPrefab;

    //Aim
    [SerializeField] Image aimForceBar;

    // FPS
    [SerializeField] private TMP_Text fpsText;

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

        sanityOriginalScale = sanityBar.transform.localScale;
        staminaOriginalScale = staminaBar.transform.localScale;
        healthOriginalScale = healthBar.transform.localScale;
        hungerOriginalScale = hungerBar.transform.localScale;

        HideItemName();
        HideItemHealth();
        HideCircularSlider();

        fpsText.gameObject.SetActive(false);
    }

    void Update()
    {     
        //HandleBeat();
        UpdateStatsBar();
        UpdateAimBar();
        UpdateFPS();
    }

    private void HandleBeat()
    {
        UpdateBeatSpeed();

        sanityBar.transform.localScale = sanityOriginalScale;
        staminaBar.transform.localScale = staminaOriginalScale;
        healthBar.transform.localScale = healthOriginalScale;
        hungerBar.transform.localScale = hungerOriginalScale;

        Beat(sanityBar, sanityOriginalScale, sanityBeatSpeed);
        Beat(staminaBar, staminaOriginalScale, staminaBeatSpeed);
        Beat(healthBar, healthOriginalScale, healthBeatSpeed);
        Beat(hungerBar, hungerOriginalScale, hungerBeatSpeed);
    }

    private void Beat(Transform stat, Vector3 originalScale, float speed)
    {
        float pulse = Mathf.Sin(Time.time * speed) * intensity;
        stat.localScale = originalScale + originalScale * pulse;
    }

    private void UpdateBeatSpeed()
    {   
        float healthPercent = player.GetCurrentHealth() / player.GetMaxHealth();
        float sanityPercent = player.GetCurrentSanity() / player.GetMaxSanity();
        float staminaPercent = player.GetCurrentStamina() / player.GetMaxStamina();
        float hungerPercent = player.GetCurrentHunger() / player.GetMaxHunger();    
        
        healthBeatSpeed = CalculateBeatSpeed(healthPercent);
        sanityBeatSpeed = CalculateBeatSpeed(sanityPercent);
        hungerBeatSpeed = CalculateBeatSpeed(hungerPercent);

        if(staminaPercent == 0f)
            staminaBeatSpeed = defaultBeatSpeed;
        else 
            staminaBeatSpeed = 0f;
    }

    private float CalculateBeatSpeed(float percent)
    {
        if(percent < 0.15f)
            return defaultBeatSpeed * 3;

        if(percent < 0.3f)
            return defaultBeatSpeed * 2;

        if(percent < 0.5f)
            return defaultBeatSpeed;

        return 0f;
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

    public void ShowItemName(string text)
    {
        itemName.text = text;
    }

    public void HideItemName()
    {
        itemName.text = "";
    }

    public void ShowItemHealth(float current, float max)
    {
        itemHealth.transform.parent.gameObject.SetActive(true);

        itemHealth.text = current + "/" + max;
        itemHealthSlider.fillAmount = current / max;
    }

    public void HideItemHealth()
    {
        itemHealth.transform.parent.gameObject.SetActive(false);
    }

    public void HideCircularSlider()
    {
        circularSlider.transform.parent.gameObject.SetActive(false);
    }

    public void ShowCircularSlider(float currentValue, bool delay)
    {  
        float startTime = 0;
        if(delay) startTime = 0.2f;

        if(currentValue > startTime)
        {   
            circularSlider.transform.parent.gameObject.SetActive(true);
            circularSlider.value = currentValue;
        }
        else{
            circularSlider.transform.parent.gameObject.SetActive(false);
        }
    }

    public void ClearKeys()
    {
        foreach (Transform child in keysLayout)
            Destroy(child.gameObject);
    }

    public void AddKey(string id, string text)
    {   
        var key = Instantiate(keyInfoPrefab, keysLayout);
        key.GetComponentInChildren<TMP_Text>().text = text;
        key.GetComponentInChildren<Image>().sprite = KeyDataBase.instance.GetIcon(id);
    }

    public void UpdateAimBar()
    {   
        if (ArmController.instance.IsAiming())
        {
            aimForceBar.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            aimForceBar.transform.parent.gameObject.SetActive(false);
        }

        GameObject item = HotBarController.instance.GetHandItem();

        if(item == null) return;
        
        if(item.TryGetComponent(out IAim aimItem))
        {
            aimForceBar.fillAmount = aimItem.GetCurrentForce()/aimItem.GetMaxForce();
        }
    }

    private void UpdateFPS()
    {
        float fps = 1f / Time.deltaTime;
        fpsText.text = "FPS: " + Mathf.RoundToInt(fps);
    }

    public void ToggleFPS()
    {
        if (fpsText.gameObject.activeSelf)
        {
            print("False");
            fpsText.gameObject.SetActive(false);
        }
        else
        {
            print("True");
            fpsText.gameObject.SetActive(true);
        }
    }
}