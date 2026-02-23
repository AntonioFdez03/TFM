using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{   
    public static CameraController playerCameraInstance;

    [SerializeField] float sensibility = 10f;
    
    private InputAction look;
    private float valueX;
    private float valueY;
    private float rotationV;

    void Awake()
    {
        if(playerCameraInstance != null && playerCameraInstance != this)
        {
            Destroy(gameObject);
            return;
        }
        playerCameraInstance = this;
    }

    void Start()
    {
        look = InputSystem.actions.FindAction("Look");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if(UIController.instance.currentState == UIState.Gameplay){
            Vector2 mouseCoords = look.ReadValue<Vector2>();
            valueX = mouseCoords.x * sensibility * Time.deltaTime;
            valueY = mouseCoords.y * sensibility * Time.deltaTime;

            rotationV = math.clamp(rotationV - valueY,-90,90);

            transform.localRotation = Quaternion.Euler(rotationV,0,0);
            
            PlayerController.playerInstance.transform.Rotate(Vector3.up * valueX);         
        }
    }
}
