using UnityEngine;
using System.Collections;
using UnityEditor;

public class ArmController : MonoBehaviour
{   
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
    private float punchForwardDistance = 0.4f;
    private float punchBackDuration = 0.3f;
    private float punchForwardDuration = 0.1f;
    private float punchReturnDuration = 0.5f;
    private float punchReturnCooldown = 0.2f;
    private float punchCooldown = 0.2f;
    private Vector3 initialLocalPos;

    void Start()
    {
        initialLocalPos = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    public void PlayAttackAnimation()
    {
        if (!isMoving)
        {   
            ItemBehaviour itemBehaviour = HotBarController.instance.GetCurrentItemBehaviour();
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

        float time = 0f;

        // 1. Retroceso
        while (time < punchBackDuration)
        {
            transform.localPosition = Vector3.Lerp(
                initialLocalPos, backPos, time / punchBackDuration
            );
            time += Time.deltaTime;
            yield return null;
        }

        // 2. Golpe hacia delante
        time = 0f;
        while (time < punchForwardDuration)
        {
            transform.localPosition = Vector3.Lerp(
                backPos, forwardPos, time / punchForwardDuration
            );
            time += Time.deltaTime;
            yield return null;
        }
       
        Punch();
        yield return new WaitForSeconds(punchReturnCooldown);
        // 3. Cooldown antes de retroceder
        // 4. Retorno
        time = 0f;
        while (time < punchReturnDuration)
        {
            transform.localPosition = Vector3.Lerp(
                forwardPos, initialLocalPos, time / punchReturnDuration
            );
            time += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(punchCooldown);

        transform.localPosition = initialLocalPos;
        isMoving = false;
    }

    private void SwingHit()
    {
        ItemBehaviour item = HotBarController.instance.GetCurrentItemBehaviour();
        if(item != null)
            item.GetComponent<ItemBehaviour>().Use();
    }

    private void Punch()
    {
        Ray ray = new Ray(CameraController.instance.transform.position, CameraController.instance.transform.transform.forward);
        RaycastHit hit;

        print("Puñetazo");
        if (Physics.Raycast(ray, out hit, punchRange))
        {
            Enemy enemy = hit.collider.CompareTag("Enemy") ? hit.collider.GetComponent<Enemy>() : null;
            if (enemy != null)
            {
                print("Enemy golpeado");
                enemy.TakeDamage(punchDamage);
            }        
        }
    }


}