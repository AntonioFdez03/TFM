using TMPro;
using UnityEngine;

public class DayCycleController : MonoBehaviour
{   
    public static DayCycleController instance;
    [SerializeField] [Range(0.0f,24f)] float currentHour = 10;
    [SerializeField] Transform sun;
    [SerializeField] float dayDuration = 24; //En minutos
    [SerializeField] float intensity = 1;
    private float sunRotationX;

    [SerializeField] private TMP_Text dayText;
    private int currentDay = 1;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void Initialize(int day, float hour)
    {
        print("Day: " + day);
        print("Current hour: " + hour);
        currentDay = day;
        currentHour = hour;
    }

    public int GetCurrentDay() => currentDay;
    public float GetCurrentHour() => currentHour;

    void Update()
    {   
        currentHour += 24/(60*dayDuration) * Time.deltaTime;

        if (currentHour >= 24f)
        {
            currentHour -= 24f;
            currentDay++;
        }

        currentHour %= 24;
        
        dayText.text = "Day \n" + currentDay.ToString();

        sunRotationX = 15 * currentHour;
        sun.localEulerAngles = new Vector3(sunRotationX,0,0);

        if(IsNight())
            sun.GetComponent<Light>().intensity = 0;
        else
            sun.GetComponent<Light>().intensity = intensity;
    }

    public bool IsNight() => currentHour < 6 || currentHour > 18;
}
