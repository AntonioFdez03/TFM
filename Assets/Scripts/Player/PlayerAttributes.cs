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

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {   
        HandleStamina();
        UpdateUI();
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable)
            return;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        StartCoroutine(InvulnerabilityCooldown());
        if(currentHealth == 0)
            PlayerController.playerInstance.SetIsDead(true);
    }
    public void HandleHunger()
    {
        
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

    IEnumerator InvulnerabilityCooldown()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }

    private void UpdateUI()
    {
        if (healthBar != null)
            healthBar.fillAmount = currentHealth / maxHealth;
        if (staminaBar != null)
            staminaBar.fillAmount = currentStamina / maxStamina;
    }
}
