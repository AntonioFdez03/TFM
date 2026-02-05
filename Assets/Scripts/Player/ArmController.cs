using UnityEngine;
using System.Collections;

public class ArmController : MonoBehaviour
{   
    [SerializeField] HotBarController hotBarController;
    [Header("Swing settings")]
    private float swingAngle = -40f;   // Ángulo de inclinación (X)
    private float swingDuration = 0.5f; // Velocidad de subida
    private float returnDuration = 0.1f; // Velocidad de bajada
    private float punchDamage = 5f;
    private float punchRange = 5f;
    private Camera playerCamera;
    
    private Quaternion initialRotation;
    private bool isSwinging = false;

    void Start()
    {
        initialRotation = transform.localRotation;
        playerCamera = Camera.main;
    }

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
        SwingHit();
    }

    private void SwingHit()
    {
        ItemBehaviour item = hotBarController.GetCurrentItemBehaviour();
        if(item != null)
            item.GetComponent<ItemBehaviour>().Use();
        else
            Punch();
    }

    private void Punch()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * punchRange, Color.red);
        if (Physics.Raycast(ray, out hit, punchRange))
        {
            Enemy enemy = hit.collider.CompareTag("Enemy") ? hit.collider.GetComponent<Enemy>() : null;
            if (enemy != null)
                enemy.TakeDamage(punchDamage);
        }
    }
}