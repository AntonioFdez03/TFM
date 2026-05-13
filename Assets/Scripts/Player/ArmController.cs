using UnityEngine;
using System.Collections;
using UnityEditor;

public class ArmController : MonoBehaviour
{   
    public static ArmController instance;

    [SerializeField] private Transform handSlot;
    [SerializeField] private Animator animator;

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
    private Vector3 initialPosition;

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
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;

        initialHandRotation = handSlot.localRotation;
        targetHandRotation = initialHandRotation * Quaternion.Euler(90f, 0f, 0f);
    }

    void Update()
    {
        UpdateAnimation();
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
            {
                animator.SetTrigger("Punch");
                StartCoroutine(PunchMovementCR());
            }

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
            Animal animal = hit.collider.CompareTag("Animal") ? hit.collider.GetComponent<Animal>() : null;
            if (animal != null)
            {
                animal.TakeDamage(punchDamage);

                Vector3 dir =
                    (animal.transform.position - transform.position).normalized;

                animal.StartCoroutine(
                    animal.KnockbackCR(dir, 0.8f, 0.15f)
                );
            }

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

        Vector3 backPos = initialPosition + Vector3.back * punchBackDistance;
        Vector3 forwardPos = initialPosition + Vector3.forward * punchForwardDistance;

        float time = 0f;

        // 1. Retroceso
        while (time < punchBackDuration)
        {
            transform.localPosition = Vector3.Lerp(
                initialPosition, backPos, time / punchBackDuration
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
                forwardPos, initialPosition, time / punchReturnDuration
            );
            time += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(punchCooldown);

        transform.localPosition = initialPosition;
        isMoving = false;
    }

    public IEnumerator AxeSwingCR()
    {
        isMoving = true;

        Quaternion prepRot = initialRotation * Quaternion.Euler(0f, 0f, -50f);

        float elapsed = 0f;

        while (elapsed < 0.08f)
        {
            float t = elapsed / 0.08f;
            transform.localRotation = Quaternion.Slerp(initialRotation, prepRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Calculamos la rotación de "golpe" sumando el ángulo al eje X
        Quaternion targetRotation = prepRot * Quaternion.Euler(swingAngle, 0, 0);
        elapsed = 0f;
        // 1. Fase de subida 
        while (elapsed < swingDuration)
        {
            transform.localRotation = Quaternion.Slerp(prepRot, targetRotation, elapsed / swingDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 2. Fase de bajada
        Quaternion hitRot = Quaternion.Euler(0, -40, -65);
        elapsed = 0;
        while (elapsed < returnDuration)
        {
            transform.localRotation = Quaternion.Slerp(targetRotation, hitRot, elapsed / returnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        ItemHit();
        
        yield return new WaitForSeconds(0.3f);
        elapsed = 0;
        while (elapsed < 0.5f)
        {
            transform.localRotation = Quaternion.Slerp(hitRot, initialRotation, elapsed / 0.5f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Aseguramos que vuelva exactamente a la posición original
        transform.localRotation = initialRotation;
        isMoving = false;
    }

    public IEnumerator PickaxeSwingCR()
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

        Vector3 backPos = initialPosition + Vector3.back * punchBackDistance;
        Vector3 forwardPos = initialPosition + Vector3.forward * punchForwardDistance;

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

        ItemHit();
        yield return new WaitForSeconds(punchReturnCooldown);

        // 3. Volver a posición y rotación inicial
        time = 0f;
        while (time < punchReturnDuration)
        {
            float t = time / punchReturnDuration;

            transform.localPosition = Vector3.Lerp(forwardPos, initialPosition, t);

            handSlot.localRotation = Quaternion.Lerp(targetHandRotation, initialHandRotation, t);

            time += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(punchCooldown);

        transform.localPosition = initialPosition;
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
        canAttack = true;
        
        while (time < 1f)
        {
            float t = time / punchReturnDuration;

            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, t);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, initialRotation, t);
            handSlot.localRotation = Quaternion.Lerp(handSlot.localRotation, initialHandRotation, t);

            time += Time.deltaTime;
            yield return null;
        }

        isMoving = false;
    }


    private void UpdateAnimation()
    {
        ItemBehaviour currentBehaviour = HotBarController.instance.GetCurrentItemBehaviour();

        if(currentBehaviour == null)
        {
            animator.SetBool("Clutch",false);
            animator.SetBool("Grab",false);
            return;
        }

        if(currentBehaviour is ToolBehaviour)
        {
            animator.SetBool("Clutch",true);
            animator.SetBool("Grab",false);
        }
        else
        {   
            animator.SetBool("Grab",true);
            animator.SetBool("Clutch",false);
        } 
    }
}