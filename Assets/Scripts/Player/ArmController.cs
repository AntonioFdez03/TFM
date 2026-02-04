using UnityEngine;
using System.Collections;

public class ArmController : MonoBehaviour
{
    [Header("Attack settings")]
    [SerializeField] private float swingAngle = -40f;   // Ángulo de inclinación (X)
    [SerializeField] private float swingDuration = 0.1f; // Velocidad de bajada
    [SerializeField] private float returnDuration = 0.15f; // Velocidad de subida
    
    private Quaternion initialRotation;
    private bool isSwinging = false;

    void Start()
    {
        // Guardamos la rotación que le diste en el Inspector como base
        initialRotation = transform.localRotation;
    }

    // Esta función la llamarás desde PlayerInteraction
    public void PlayAttackAnimation()
    {
        if (!isSwinging)
        {
            StartCoroutine(SwingCoroutine());
        }
    }

    private IEnumerator SwingCoroutine()
    {
        isSwinging = true;

        // Calculamos la rotación de "golpe" sumando el ángulo al eje X
        Quaternion targetRotation = initialRotation * Quaternion.Euler(swingAngle, 0, 0);

        // 1. Fase de Bajada (Golpe)
        float elapsed = 0;
        while (elapsed < swingDuration)
        {
            transform.localRotation = Quaternion.Slerp(initialRotation, targetRotation, elapsed / swingDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 2. Fase de Subida (Retorno)
        elapsed = 0;
        while (elapsed < returnDuration)
        {
            transform.localRotation = Quaternion.Slerp(targetRotation, initialRotation, elapsed / returnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Aseguramos que vuelva exactamente a la posición original
        transform.localRotation = initialRotation;
        isSwinging = false;
    }
}