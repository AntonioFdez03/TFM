using System;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{   
    [Header("References")]
    [SerializeField] Transform playerCamera;
    private PlayerAttributes playerAttributes;
    private CharacterController controller;

    [Header("Movement")]
    [SerializeField] float movementSpeed = 10f; 
    [SerializeField] float sprintSpeed = 20f; 
    [SerializeField] float jumpForce = 10f;
    private InputAction move;
    private InputAction sprint;
    private InputAction jump;

    [Header("Gravity")]
    private Vector3 gravity = Vector3.down * 30f;
    private float yVelocity;

    void Start()
    {   
        //References
        controller = GetComponent<CharacterController>();
        playerAttributes = GetComponent<PlayerAttributes>();

        //Actions
        move = InputSystem.actions.FindAction("Move");
        sprint = InputSystem.actions.FindAction("Sprint");
        jump = InputSystem.actions.FindAction("Jump");
    }   

    void Update()
    {
        Vector3 finalMovement = Vector3.zero;

        finalMovement += CalculateHorizontalMovement();
        finalMovement += CalculateVerticalMovement();
        controller.Move(finalMovement * Time.deltaTime);
    }

    private Vector3 CalculateHorizontalMovement()
    {
        Vector2 playerInput = move.ReadValue<Vector2>();
        Vector3 cameraForward = playerCamera.forward;
        Vector3 cameraRight = playerCamera.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 direction = cameraRight * playerInput.x + cameraForward * playerInput.y;
        direction.Normalize();

        bool isMoving = direction.sqrMagnitude > 0.0001f;
        bool isSprinting = isMoving && playerAttributes.canSprint && sprint.IsPressed() && controller.isGrounded;

        if (isSprinting) playerAttributes.UseStamina();

        float currentSpeed = isSprinting ? sprintSpeed : movementSpeed;
        
        return direction * currentSpeed;
    }

    private Vector3 CalculateVerticalMovement()
    {
        //Estabilizar grounded para que no de fallos
        if (controller.isGrounded && yVelocity < 0)
        {
            yVelocity = -2f; 
            controller.stepOffset = 0.3f;
        }

        // SALTO
        if (jump.triggered && controller.isGrounded)
        { 
            yVelocity = jumpForce;
            controller.stepOffset = 0;
        }

        // Aplicar gravedad a la variable persistente
        yVelocity += gravity.y * Time.deltaTime;

        // Devolvemos el vector vertical
        return new Vector3(0, yVelocity, 0);
    }
}
