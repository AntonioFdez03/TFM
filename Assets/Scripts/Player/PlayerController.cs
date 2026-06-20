using System;
using System.Collections;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{   
    public static PlayerController instance;

    private PlayerAttributes playerAttributes;
    private CharacterController controller;

    private float movementSpeed = 13f; 
    private float sprintSpeed = 20f; 
    private float crouchSpeed = 7f;
    private float jumpForce = 10f;

    private bool canJump;
    private bool canMove;
    private bool isDead;
    private bool isMoving;
    private bool isSprinting;
    private bool isCrouching;
    private bool isJumping;

    private InputAction move;
    private InputAction sprint;
    private InputAction jump;
    private InputAction crouch;

    private Vector3 gravity = Vector3.down * 30f;
    private float yVelocity;
    private Vector3 groundNormal;

    private Vector3 initialCameraPosition;
    private Vector3 crouchCameraPosition;

    private float fallTimer = 0;
    private float fallLimit = 1.5f;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        canMove = true;
        
        controller = GetComponent<CharacterController>();
        playerAttributes = GetComponent<PlayerAttributes>();

        move = InputSystem.actions.FindAction("Move");
        sprint = InputSystem.actions.FindAction("Sprint");
        jump = InputSystem.actions.FindAction("Jump");
        crouch = InputSystem.actions.FindAction("Crouch");

        initialCameraPosition = Camera.main.transform.localPosition;
        crouchCameraPosition = initialCameraPosition;
        crouchCameraPosition.y = initialCameraPosition.y/2;
    }

    public void InitializePlayer(Vector3 position, Quaternion rotation, float health, float hunger, float stamina, float sanity)
    {
        transform.position = position;
        transform.rotation = rotation;
        playerAttributes.SetAttributes(health,hunger,stamina, sanity);
    }   
    
    public PlayerAttributes GetPlayerAttributes() => playerAttributes;
    public void SetCanMove(bool cM) => canMove = cM;
    public bool CanMove() => canMove;
    public bool IsMoving() => isMoving;
    public bool IsSprinting() => isSprinting;
    public bool IsCrouching() => isCrouching;
    public bool IsDead() => isDead;

    void Update()
    {   
        if (!isDead)
        {  
            float angle = Vector3.Angle(groundNormal, Vector3.up);

            if (angle > controller.slopeLimit)
            {
                Vector3 slide = new Vector3(groundNormal.x, -groundNormal.y, groundNormal.z);
                controller.Move(slide * 5f * Time.deltaTime);
            }

            if (canMove)
            {
                Crouch();

                Vector3 finalMovement = Vector3.zero;
                finalMovement += CalculateHorizontalMovement();
                finalMovement += CalculateVerticalMovement();

                controller.Move(finalMovement * Time.deltaTime);
            }
        }
    }

    private Vector3 CalculateHorizontalMovement()
    {
        Vector2 playerInput = move.ReadValue<Vector2>();
        Vector3 cameraForward = CameraController.instance.transform.forward;
        Vector3 cameraRight = CameraController.instance.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 direction = cameraRight * playerInput.x + cameraForward * playerInput.y;
        direction.Normalize();

        isMoving = direction.sqrMagnitude > 0.0001f;
        isSprinting = isMoving && playerAttributes.canSprint && sprint.IsPressed();

        if (isSprinting) playerAttributes.UseStamina();

        float currentSpeed = isCrouching ? crouchSpeed : isSprinting? sprintSpeed : movementSpeed;
        
        if(isJumping && isSprinting)
            currentSpeed *= 0.75f;

        return direction * currentSpeed;
    }

    private Vector3 CalculateVerticalMovement()
    {
        if (!controller.isGrounded)
        {
            fallTimer += Time.deltaTime;
            if(fallTimer >= fallLimit)
                playerAttributes.TakeDamage(3*fallTimer);
        }else
            fallTimer = 0;

        //Estabilizar grounded para que no de fallos
        if (controller.isGrounded && yVelocity < 0)
        {   
            canJump = true;
            isJumping = false;
            yVelocity = -2f; 
            controller.stepOffset = 0.3f;
        }

        if (jump.triggered && canJump)
        {   
            canJump = false;
            isJumping = true;
            AudioManager.instance.PlayOneShot("PlayerJump");
            yVelocity = jumpForce;
            controller.stepOffset = 0;
        }

        yVelocity += gravity.y * Time.deltaTime;
        return new Vector3(0, yVelocity, 0);
    }

    private void Crouch()
    {
        isCrouching = crouch.IsPressed();

        Vector3 crouchCameraPosition = initialCameraPosition;
        crouchCameraPosition.y = initialCameraPosition.y / 2f;

        Vector3 targetPosition = isCrouching ? crouchCameraPosition : initialCameraPosition;

        Camera.main.transform.localPosition = Vector3.Lerp( Camera.main.transform.localPosition, targetPosition, 8f * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        groundNormal = hit.normal;
    }

    public void Die()
    {   
        string savePath = Application.persistentDataPath + "/save.json";
        if(File.Exists(savePath))
            File.Delete(savePath);

        if(isDead) return;

        isDead = true;
        canMove = false;
        isMoving = false;

        AudioManager.instance.PlayOneShot("PlayerDeath");
        StartCoroutine(DeathCR());
    }

    private IEnumerator DeathCR()
    {
        GameplayUI.instance.HideUI();

        // Esperar a aterrizar
        while(!controller.isGrounded)
        {
            yVelocity += gravity.y * Time.deltaTime;
            controller.Move(new Vector3(0, yVelocity, 0) * Time.deltaTime);

            yield return null;
        }

        yield return new WaitForSeconds(0.4f);

        Transform cam = Camera.main.transform;

        // ===== FASE 1: Se desploma =====

        Vector3 camStartPos = cam.localPosition;
        Vector3 camKneelPos = camStartPos + Vector3.down * 0.7f;

        float timer = 0f;
        float kneelDuration = 0.4f;

        ScreenEffectsController.instance.PlayDeathEffect();
        while(timer < kneelDuration)
        {
            timer += Time.deltaTime;

            float t = timer / kneelDuration;
            t = 1f - Mathf.Pow(1f - t, 2f);

            cam.localPosition = Vector3.Lerp(
                camStartPos,
                camKneelPos,
                t
            );

            yield return null;
        }

        // Ya no necesitamos el CharacterController
        controller.enabled = false;

        // ===== FASE 2: Cae =====
        Quaternion startRot = transform.rotation;
        Quaternion targetRot =
            startRot * Quaternion.Euler(85f, 0f, 40f);

        timer = 0f;
        float fallDuration = 0.8f;

        while(timer < fallDuration)
        {
            timer += Time.deltaTime;

            float t = timer / fallDuration;
            t = t * t;

            transform.rotation = Quaternion.Slerp(
                startRot,
                targetRot,
                t
            );

            yield return null;
        }
    }
}
