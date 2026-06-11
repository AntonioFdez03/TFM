using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{   
    public static CameraController instance;

    [SerializeField] float sensibility = 10f;
    
    private InputAction look;
    private float valueX;
    private float valueY;
    private float rotationV;

    //Balanceo al moverse
    private float bobAmount = 0.05f;
    private float bobSpeed = 8f;
    private float runMultiplier = 1.6f;

    private Vector3 originalCamPos;
    private float bobTimer;

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
        look = InputSystem.actions.FindAction("Look");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalCamPos = transform.localPosition;
    }

    public float GetCurrentRotation() => rotationV;
    public void SetCurrentRotation(float rotation) => rotationV = rotation;

    void Update()
    {
        if(UIController.instance.GetCurrentState() == UIState.Gameplay){
            Vector2 mouseCoords = look.ReadValue<Vector2>();
            valueX = mouseCoords.x * sensibility * Time.deltaTime;
            valueY = mouseCoords.y * sensibility * Time.deltaTime;

            rotationV = math.clamp(rotationV - valueY,-90,90);

            transform.localRotation = Quaternion.Euler(rotationV,0,0);
            
            PlayerController.instance.transform.Rotate(Vector3.up * valueX);      
            //HandleHeadBob();   
        }
    }

    private void HandleHeadBob()
    {
        bool isMoving = PlayerController.instance.IsMoving(); // ajusta a tu sistema

        if (!isMoving)
        {
            bobTimer = 0f;
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                originalCamPos,
                Time.deltaTime * 8f
            );
            return;
        }

        float speed = bobSpeed * (PlayerController.instance.IsSprinting() ? runMultiplier : 1f);

        bobTimer += Time.deltaTime * speed;

        float x = Mathf.Sin(bobTimer) * bobAmount;
        float y = Mathf.Cos(bobTimer * 2f) * bobAmount * 0.5f;

        Vector3 bobOffset = new Vector3(x, y, 0);

        transform.localPosition = originalCamPos + bobOffset;
    }
}
