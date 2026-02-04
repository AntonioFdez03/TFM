using System.Collections;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{   
    [Header("Health attributes")]
    [SerializeField] UnityEngine.UI.Image healthBar;
    [SerializeField] float currentHealth;
    [SerializeField] float maxHealth = 100f;
    private float invulnerabilityDuration = 1f;
    private bool isInvulnerable = false;

    [Header("Stamina attributes")]
    [SerializeField] UnityEngine.UI.Image staminaBar;
    [SerializeField] float currentStamina;
    [SerializeField] float maxStamina = 100f;
    [SerializeField] float burnRate = 10f; //Consumo por segundo
    [SerializeField] float recoveryRate = 15f; //Recuperación por segundo
    [SerializeField] float recoveryDelay = 1f;
    [SerializeField] float timeSinceLastSprint = 0f;
    public bool canSprint;

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
    public void UseStamina()
    {
        currentStamina -= burnRate * Time.deltaTime;
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

    public void TakeDamage(float damage)
    {
        if (isInvulnerable)
            return;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        StartCoroutine(InvulnerabilityCooldown());
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
