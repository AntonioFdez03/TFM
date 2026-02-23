using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{   
    [SerializeField] float sensibility = 10f;
    [SerializeField] Transform player;
    
    private InputAction look;
    private float valueX;
    private float valueY;
    private float rotationV;

    void Start()
    {
        look = InputSystem.actions.FindAction("Look");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(UIController.instance.currentState == UIController.UIState.Gameplay){
            Vector2 mouseCoords = look.ReadValue<Vector2>();
            valueX = mouseCoords.x * sensibility * Time.deltaTime;
            valueY = mouseCoords.y * sensibility * Time.deltaTime;

            rotationV = math.clamp(rotationV - valueY,-90,90);

            transform.localRotation = Quaternion.Euler(rotationV,0,0);
            
            if(player != null){
                player.Rotate(Vector3.up * valueX);
            }
        }
    }
}
