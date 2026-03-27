using UnityEngine;
using System.Collections;
using UnityEditor;

public class ArmController : MonoBehaviour
{   
    public static ArmController instance;
    [SerializeField] private Transform handSlot;
    private Quaternion initialHandRotation;
    private Quaternion targetHandRotation;
    private bool isMoving = false;
    private float AttackCooldown = 0.9f;
    private bool canAttack = true;

    //Swing settings
    private float swingAngle = -40f;   // Ángulo de inclinación (X)
    private float swingDuration = 0.5f; // Velocidad de subida
    private float returnDuration = 0.1f; // Velocidad de bajada
    private Quaternion initialRotation;

    //Punch movement settings
    private float punchDamage = 5f;
    private float punchRange = 5f;
    private float punchBackDistance = 0.1f;
    private float punchForwardDistance = 0.6f;
    private float punchBackDuration = 0.15f;
    private float punchForwardDuration = 0.1f;
    private float punchReturnDuration = 0.5f;
    private float punchReturnCooldown = 0.2f;
    private float punchCooldown = 0.2f;
    private Vector3 initialLocalPos;

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
        initialLocalPos = transform.localPosition;
        initialRotation = transform.localRotation;

        initialHandRotation = handSlot.localRotation;
        targetHandRotation = initialHandRotation * Quaternion.Euler(90f, 0f, 0f);
    }

    public bool IsMoving() => isMoving;
    public bool CanAttack() => canAttack;

    public void PlayAttackAnimation()
    {
        if (!isMoving && canAttack)
        {
            ItemBehaviour item = HotBarController.instance.GetCurrentItemBehaviour();

            if(item != null)
                item.Attack(this);
            else
                StartCoroutine(PunchMovementCR());

            StartCoroutine(AttackCooldownCR());
        }
    }

    private void ItemHit()
    {
        ItemBehaviour item = HotBarController.instance.GetCurrentItemBehaviour();
        if(item != null)
            item.GetComponent<ItemBehaviour>().Use();
    }

    private void Punch()
    {
        Ray ray = new Ray(CameraController.instance.transform.position, CameraController.instance.transform.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, punchRange))
        {
            Enemy enemy = hit.collider.CompareTag("Enemy") ? hit.collider.GetComponent<Enemy>() : null;
            if (enemy != null)
                enemy.TakeDamage(punchDamage);

            HarvestableObject harvestableObject = hit.collider.CompareTag("Harvestable") ? hit.collider.GetComponent<HarvestableObject>() : null;
            if(harvestableObject != null)
            {
                harvestableObject.TakeHit(ToolType.None,punchDamage);
                PlayerAttributes player = PlayerController.instance.GetPlayerAttributes();
                player.TakeDamage(2f);
            }  
        }
    }

    public IEnumerator PunchMovementCR()
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

    public IEnumerator ToolSwingCR()
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
        ItemHit();
    }

    public IEnumerator SpearMovementCR()
    {
        isMoving = true;

        Vector3 backPos = initialLocalPos + Vector3.back * punchBackDistance;
        Vector3 forwardPos = initialLocalPos + Vector3.forward * punchForwardDistance;

        float time = 0f;

        // 1. Retroceso + rotación hacia atrás
        while (time < punchBackDuration)
        {
            float t = time / punchBackDuration;

            ///transform.localPosition = Vector3.Lerp(initialLocalPos, backPos, t);

            handSlot.localRotation = Quaternion.Slerp(initialHandRotation, targetHandRotation, t);

            time += Time.deltaTime;
            yield return null;
        }

        // 2. Golpe hacia delante
        time = 0f;
        while (time < punchForwardDuration)
        {
            float t = time / punchForwardDuration;

            transform.localPosition = Vector3.Lerp(backPos, forwardPos, t);

            time += Time.deltaTime;
            yield return null;
        }

        print("Golpea");
        ItemHit();
        print("Vuelve");
        yield return new WaitForSeconds(punchReturnCooldown);

        // 3. Volver a posición y rotación inicial
        time = 0f;
        while (time < punchReturnDuration)
        {
            float t = time / punchReturnDuration;

            transform.localPosition = Vector3.Lerp(forwardPos, initialLocalPos, t);

            handSlot.localRotation = Quaternion.Lerp(targetHandRotation, initialHandRotation, t);

            time += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(punchCooldown);

        transform.localPosition = initialLocalPos;
        handSlot.localRotation = initialHandRotation;

        isMoving = false;
    }

    IEnumerator AttackCooldownCR()
    {
        canAttack = false;
        yield return new WaitForSeconds(AttackCooldown);
        canAttack = true;
    }

    public void ResetArm()
    {
        StopAllCoroutines();
        StartCoroutine(ResetArmCR());
    }

    IEnumerator ResetArmCR()
    {   
        float time = 0f;
        while (time < 1f)
        {
            float t = time / punchReturnDuration;

            transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPos, t);
            handSlot.localRotation = Quaternion.Lerp(handSlot.localRotation, initialHandRotation, t);

            time += Time.deltaTime;
            yield return null;
        }

        canAttack = true;
        isMoving = false;
    }
}