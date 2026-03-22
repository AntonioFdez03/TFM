using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{   
    //Health 
    [SerializeField] UnityEngine.UI.Image healthBar;
    private float currentHealth;
    private float maxHealth = 100f;
    private float invulnerabilityDuration = 1f;
    private bool isInvulnerable = false;

    //Stamina
    [SerializeField] UnityEngine.UI.Image staminaBar;
    private float currentStamina;
    private float maxStamina = 100f;
    private float staminaBurnRate = 10f; //Consumo por segundo
    private float recoveryRate = 15f; //Recuperación por segundo
    private float recoveryDelay = 1f;
    private float timeSinceLastSprint = 0f;
    public bool canSprint;

    //Hunger
    [SerializeField] UnityEngine.UI.Image hungerBar;
    private float currentHunger;
    private float maxHunger = 100f;
    private float hungerBurnRate = 1f;
    private float timeSinceLastHungerDecrase = 0f;
    private float hungerDecreaseInterval = 10f;

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentHunger = maxHunger;
    }

    // Update is called once per frame
    void Update()
    {   
        HandleHunger();
        HandleStamina();
        UpdateUI();
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable)
            return;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        StartCoroutine(DamageCooldown(invulnerabilityDuration));
        if(currentHealth == 0)
            PlayerController.instance.SetIsDead(true);
    }
    public void HandleHunger()
    {
        timeSinceLastHungerDecrase += Time.deltaTime;

        if(PlayerController.instance.IsSprinting())
            hungerDecreaseInterval = 5f;
        else
            hungerDecreaseInterval = 10f;

        if(timeSinceLastHungerDecrase >= hungerDecreaseInterval)
        {
            timeSinceLastHungerDecrase = 0f;
            currentHunger -= hungerBurnRate;
        }

        currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);
    }

    public void Eat(float amount)
    {
        currentHunger = Math.Clamp(currentHunger + amount, 0, maxHunger);
    }

    public void UseStamina()
    {
        currentStamina -= staminaBurnRate * Time.deltaTime;
        timeSinceLastSprint = 0f;
    }

    public void HandleStamina()
    {
        canSprint = currentStamina > 0.01f;
        timeSinceLastSprint += Time.deltaTime;

        if (timeSinceLastSprint >= recoveryDelay && currentStamina < maxStamina)
        {
            currentStamina += recoveryRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    IEnumerator DamageCooldown(float duration)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(duration);
        isInvulnerable = false;
    }

    private void UpdateUI()
    {
        if (healthBar != null)
            healthBar.fillAmount = currentHealth / maxHealth;
        if (staminaBar != null)
            staminaBar.fillAmount = currentStamina / maxStamina;
        if(hungerBar != null)
            hungerBar.fillAmount = currentHunger / maxHunger;
    }
}
