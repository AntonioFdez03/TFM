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
    private bool isInvulnerable = false;
    private float invulnerabilityDuration = 1f;
    private bool canHeal = true;
    private float healingCooldown = 1;

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
    private float hungerDamage = 5f;
    private float hungerHeal = 5f;

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
        HandleHealth();
        HandleStamina();
        UpdateUI();
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetCurrentHunger() => currentHunger;
    public float GetMaxHunger() => maxHunger;

    public void TakeDamage(float damage)
    {
        if (isInvulnerable)
            return;
        currentHealth = Mathf.Clamp(currentHealth-damage, 0f, maxHealth);
        StartCoroutine(DamageCooldownCR());
        if(currentHealth == 0)
            PlayerController.instance.SetIsDead(true);
    }

    private void HandleHealth()
    {
        if(currentHunger == 0)
            TakeDamage(hungerDamage);
        else if(canHeal && currentHunger > 0.75 * maxHunger && currentHealth < maxHealth)
        {
            currentHealth = Math.Clamp(currentHealth+hungerHeal,0,maxHealth);
            StartCoroutine(HealingCooldownCR());
        }
    }
    private void HandleHunger()
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

    private void HandleStamina()
    {
        canSprint = currentStamina > 0.01f;
        timeSinceLastSprint += Time.deltaTime;

        if (timeSinceLastSprint >= recoveryDelay && currentStamina < maxStamina)
        {
            currentStamina += recoveryRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
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

    IEnumerator HealingCooldownCR()
    {
        canHeal = false;
        yield return new WaitForSeconds(healingCooldown);
        canHeal = true;
    }

    IEnumerator DamageCooldownCR()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }
}
