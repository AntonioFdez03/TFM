using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttributes : MonoBehaviour
{   
    public event Action<float> OnHealthChanged;
    public event Action<float> OnSanityChanged;

    //Health 
    private float currentHealth;
    private float maxHealth = 100f;
    private bool isInvulnerable = false;
    private float invulnerabilityDuration = 1f;
    private bool canHeal = true;
    private float healingCooldown = 5;
    private float timeSinceLastDamage = 0f;

    //Stamina
    private float currentStamina;
    private float maxStamina = 100f;
    private float staminaBurnRate = 10f; //Consumo por segundo
    private float recoveryRate = 15f; //Recuperación por segundo
    private float recoveryDelay = 1f;
    private float timeSinceLastSprint = 0f;
    public bool canSprint;

    //Hunger
    private float currentHunger;
    private float maxHunger = 100f;
    private float hungerBurnRate = 1f;
    private float timeSinceLastHungerDecrase = 0f;
    private float hungerDecreaseInterval = 10f;
    private float hungerDamage = 5f;
    private float hungerHeal = 5f;

    //Sanity
    private float currentSanity;
    private float maxSanity = 100f;
    private float timeSinceLastSanityDecrase = 0f;
    private float sanityDecreaseInterval = 5f;
    private bool inLight = false;

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentHunger = maxHunger;
        currentSanity = maxSanity;
    }

    void Update()
    {   
        HandleHunger();
        HandleHealth();
        HandleStamina();
        HandleSanity();
    }

    public void SetAttributes(float health, float hunger, float stamina, float sanity)
    {
        currentHealth = health;
        currentHunger = hunger;
        currentStamina = stamina;
        currentSanity = sanity;
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetCurrentHunger() => currentHunger;
    public float GetMaxHunger() => maxHunger;
    public float GetCurrentStamina() => currentStamina;
    public float GetMaxStamina() => maxStamina;
    public float GetCurrentSanity() => currentSanity;
    public float GetMaxSanity() => maxSanity;
    public void SetInLight(bool value) => inLight = value;

    public void TakeDamage(float damage)
    {  
        if (isInvulnerable)
            return;
        currentHealth = Mathf.Clamp(currentHealth-damage, 0f, maxHealth);
        StartCoroutine(DamageCooldownCR());
        timeSinceLastDamage = 0f;

        if(currentHealth == 0)
            PlayerController.instance.Die();
        else
            AudioManager.instance.PlayOneShot("PlayerDamage");

        OnHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    private void HandleHealth()
    {   
        timeSinceLastDamage += Time.deltaTime;
        if(currentHunger == 0)
            TakeDamage(hungerDamage);
        else if(canHeal && currentHunger > 0.75 * maxHunger && currentHealth < maxHealth && timeSinceLastDamage > 10f)
        {
            currentHealth = Math.Clamp(currentHealth+hungerHeal,0,maxHealth);
            OnHealthChanged?.Invoke(currentHealth / maxHealth);
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

    private void HandleSanity()
    {   
        timeSinceLastSanityDecrase += Time.deltaTime;

        if(timeSinceLastSanityDecrase >= sanityDecreaseInterval)
        {
            if(!DayCycleController.instance.IsNight() || inLight)
                UpdateSanity(2f);
            else
                UpdateSanity(-2f);

            timeSinceLastSanityDecrase = 0f;
        } 
    }

    public void Eat(float amount)
    {
        currentHunger = Math.Clamp(currentHunger + amount, 0, maxHunger);
        timeSinceLastHungerDecrase = 0f;
    }

    public void UpdateHealth(float amount)
    {
        currentHealth = Math.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    public void UpdateSanity(float amount)
    {   
        currentSanity = Math.Clamp(currentSanity + amount, 0, maxSanity);
        OnSanityChanged?.Invoke(currentSanity / maxSanity);
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Light"))
        {
            inLight = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        
        if(other.CompareTag("Light"))
        {
            inLight = false;
        }
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
