using UnityEngine;

public class Helicopter : MonoBehaviour
{   
    [SerializeField] private GameObject rescueRope;
    [SerializeField] private AudioClip helicopterSound;
    [SerializeField] private bool movement = true;
    private AudioSource audioSource;
    private Vector3 helicopterTarget;
    private float speed = 40f;
    private float rotationSpeed = 2f;
    private float timer = 0;
    private bool ropeActive = false;
    private bool helicopterArrived = false;
    private bool leave = false;

    void Start()
    {   
        rescueRope.SetActive(false);

        audioSource = GetComponent<AudioSource>();

        if(movement)
            audioSource.volume = 1f;
        else
            audioSource.volume = 0.5f;
            
        audioSource.spatialBlend = 1;
        audioSource.clip = helicopterSound;
        audioSource.loop = true;
        audioSource.Play();

        if(helicopterArrived && timer >= GameController.instance.GetHelicopterWaitTime())
        {
            leave = true;
        }
    }

    public void SetTarget(Vector3 target){
        helicopterTarget = target;
        transform.LookAt(helicopterTarget);
    }

    public void Leave() => leave = true; 
    public bool TimePassed() => timer >= GameController.instance.GetHelicopterWaitTime();

    void Update()
    {
        if(movement)
        {
            float distance = 0;
        
            if(!helicopterArrived)
                distance = Vector3.Distance(transform.position, helicopterTarget);


            if (distance > 10)
                speed = 40f;
            else if (distance > 5)
                speed = 20f;
            else
            {   
                helicopterArrived = true;
                speed = 0f;

                if (!ropeActive)
                {
                    ropeActive = true;
                    rescueRope.SetActive(true);
                }

                timer += Time.deltaTime;
                ObjectivesController.instance.UpdateHelicopterTime(timer);

                if(timer >= GameController.instance.GetHelicopterWaitTime())
                {
                    if(!leave)
                        ObjectivesController.instance.NextObjective(ObjectiveType.NoEscape);

                    speed = 25f;
                }
            }

            transform.position += transform.forward * speed * Time.deltaTime;
        }

        ApplyHelicopterWobble();
    }

    private void ApplyHelicopterWobble()
    {
        // Dirección hacia el objetivo
        Vector3 dir = (helicopterTarget - transform.position).normalized;

        if(!movement || leave)
            dir = transform.forward;

        // Rotación principal hacia el objetivo
        Quaternion lookRotation = Quaternion.LookRotation(dir);

        float rollMultiply = 4f;
        float pitchMultiply = 2f;

        if (!movement)
        {
            rollMultiply = 2f;
            pitchMultiply = 1f;
        }
        // Bamboleos naturales
        float roll = Mathf.Sin(Time.time * 0.8f) * rollMultiply;     // izquierda/derecha
        float pitch = Mathf.Sin(Time.time * 1.3f) * pitchMultiply;    // adelante/atrás

        if(speed == 0)
        {
            roll = 0;
            pitch = 0;
        }

        // Movimiento vertical
        float hover = Mathf.Sin(Time.time * 1.5f) * 0.15f;

        // Aplicar rotación con balanceo
        Quaternion wobbleRotation = Quaternion.Euler(pitch, 0f, roll);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation * wobbleRotation,
            rotationSpeed * Time.deltaTime
        );

        // Aplicar pequeña oscilación vertical
        transform.position += Vector3.up * hover * Time.deltaTime;
    }

}