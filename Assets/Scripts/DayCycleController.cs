using UnityEngine;

public class DayCycleController : MonoBehaviour
{
    [SerializeField] [Range(0.0f,24f)] float currentHour;
    [SerializeField] Transform sun;
    [SerializeField] float dayDuration = 24; //En minutos
    [SerializeField] float intensity = 1;
    private float sunRotationX;

    void Start()
    {
        currentHour = 10;
    }
    void Update()
    {   
        currentHour += 24/(60*dayDuration) * Time.deltaTime;
        currentHour %= 24;

        sunRotationX = 15 * currentHour;
        sun.localEulerAngles = new Vector3(sunRotationX,0,0);

        if(currentHour < 6 || currentHour > 18)
        {
            sun.GetComponent<Light>().intensity = 0;
        }
        else
        {
            sun.GetComponent<Light>().intensity = intensity;
        }
    }
}
