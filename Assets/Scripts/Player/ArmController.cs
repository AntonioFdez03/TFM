using UnityEngine;
using System.Collections;
using UnityEditor;

public class ArmController : MonoBehaviour
{   
    [SerializeField] HotBarController hotBarController;
    private Camera playerCamera;
    private bool isMoving = false;

    //Swing settings
    private float swingAngle = -40f;   // Ángulo de inclinación (X)
    private float swingDuration = 0.5f; // Velocidad de subida
    private float returnDuration = 0.1f; // Velocidad de bajada
    private Quaternion initialRotation;

    //Punch movement settings
    private float punchDamage = 5f;
    private float punchRange = 5f;
    private float punchBackDistance = 0.2f;
    private float punchForwardDistance = 0.2f;
    private float punchBackDuration = 0.3f;
    private float punchForwardDuration = 0.05f;
    private float punchReturnDuration = 0.1f;
    private Vector3 initialLocalPos;

    void Start()
    {
        initialLocalPos = transform.localPosition;
        initialRotation = transform.localRotation;
        playerCamera = Camera.main;
    }

    public void PlayAttackAnimation()
    {
        if (!isMoving)
        {   
            ItemBehaviour itemBehaviour = hotBarController.GetCurrentItemBehaviour();
            print(itemBehaviour);
            if(itemBehaviour == null)
                StartCoroutine(PunchMovementCoroutine());  
            else if(itemBehaviour is ToolBehaviour)
                StartCoroutine(ToolSwingCoroutine());
        }
    }

    private IEnumerator ToolSwingCoroutine()
    {
        isMoving = true;

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
        isMoving = false;
        SwingHit();
    }

    private IEnumerator PunchMovementCoroutine()
    {
        isMoving = true;

        Vector3 backPos = initialLocalPos + Vector3.back * punchBackDistance;
        Vector3 forwardPos = initialLocalPos + Vector3.forward * punchForwardDistance;

        float elapsed = 0f;

        // 1. Retroceso
        while (elapsed < punchBackDuration)
        {
            transform.localPosition = Vector3.Lerp(
                initialLocalPos, backPos, elapsed / punchBackDuration
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 2. Golpe hacia delante
        elapsed = 0f;
        while (elapsed < punchForwardDuration)
        {
            transform.localPosition = Vector3.Lerp(
                backPos, forwardPos, elapsed / punchForwardDuration
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 💥 Aquí es donde debe pegar
        Punch();

        // 3. Retorno
        elapsed = 0f;
        while (elapsed < punchReturnDuration)
        {
            transform.localPosition = Vector3.Lerp(
                forwardPos, initialLocalPos, elapsed / punchReturnDuration
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = initialLocalPos;
        isMoving = false;
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