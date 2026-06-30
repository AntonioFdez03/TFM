using TMPro;
using UnityEngine;

public class DayCycleController : MonoBehaviour
{   
    public static DayCycleController instance;
    [SerializeField] [Range(0.0f,24f)] float currentHour = 10;
    [SerializeField] Transform sun;
    [SerializeField] float dayDuration = 24; //En minutos
    [SerializeField] float intensity = 1;

    [SerializeField] private Color dayAmbientLight;
    [SerializeField] private Color nightAmbientLight;
    [SerializeField] private Color dayFogColor;
    [SerializeField] private Color nightFogColor;

    private float sunRotationX;

    [SerializeField] private TMP_Text dayText;
    private int currentDay = 1;
    private int objectiveDaysSurvived = 0;

    private bool wasNight;
    private bool wasLowSanity;

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
        wasNight = IsNight();
        SetInitialAmbient();
    }

    public void Initialize(int day, float hour)
    {
        currentDay = day;
        currentHour = hour;
    }

    public void SetObjectiveDaysSurvived(int days) => objectiveDaysSurvived = days;
    public int GetObjectiveDaysSurvived() => objectiveDaysSurvived;
    public int GetCurrentDay() => currentDay;
    public float GetCurrentHour() => currentHour;

    void Update()
    {   
        currentHour += 24/(60*dayDuration) * Time.deltaTime;

        if (currentHour >= 24f)
        {
            currentHour -= 24f;
            currentDay++;

            if(ObjectivesController.instance.GetCurrentObjective() == ObjectiveType.Survive)
            {
                objectiveDaysSurvived ++;
                ObjectivesController.instance.UpdateSurviveObjective(objectiveDaysSurvived);
            }
        }

        currentHour %= 24;
        
        dayText.text = "Day \n" + currentDay.ToString();

        sunRotationX = 15 * currentHour;
        sun.localEulerAngles = new Vector3(sunRotationX,0,0);

        UpdateAmbientLight();
        UpdateAmbientSound();
    }

    public bool IsNight() => currentHour < 6 || currentHour > 20;

    private void UpdateAmbientLight()
    {
        float t = 0f;

        // Noche profunda (00:00 - 06:00)
        if (currentHour < 6f)
        {
            t = 0f;
        }

        // Amanecer (06:00 - 08:00)
        else if (currentHour < 8f)
        {
            t = Mathf.InverseLerp(
                6f,
                8f,
                currentHour
            );
        }

        // Día completo (08:00 - 18:00)
        else if (currentHour < 18f)
        {
            t = 1f;
        }

        // Atardecer (18:00 - 20:00)
        else if (currentHour < 20f)
        {
            t = 1f - Mathf.InverseLerp(
                18f,
                20f,
                currentHour
            );
        }

        // Noche (20:00 - 24:00)
        else
        {
            t = 0f;
        }

        // Luz ambiental gradual
        RenderSettings.ambientLight =
            Color.Lerp(
                nightAmbientLight,
                dayAmbientLight,
                t
            );
        
        RenderSettings.fogColor = Color.Lerp(nightFogColor, dayFogColor, t);

        // Sol gradual
        sun.GetComponent<Light>().intensity =
            Mathf.Lerp(0, intensity, t);
    }


    private void UpdateAmbientSound()
    {
         bool isNight = IsNight();
         bool isLowSanity = PlayerController.instance.GetPlayerAttributes().LowSanity();

        if (isNight != wasNight || isLowSanity != wasLowSanity)
        {
            wasNight = isNight;
            wasLowSanity = isLowSanity;

            if (PlayerController.instance.GetPlayerAttributes().LowSanity())
            {
                if (IsNight())
                    AudioManager.instance.PlayAmbient("LowSanityNight");
                else
                    AudioManager.instance.PlayAmbient("LowSanityDay");
            }
            else
            {
                if (IsNight())
                    AudioManager.instance.PlayAmbient("ForestNight");
                else
                    AudioManager.instance.PlayAmbient("ForestDay");
            }
        }
    }

    private void SetInitialAmbient()
    {   
        if (PlayerController.instance.GetPlayerAttributes().LowSanity())
        {   
            if (IsNight())
                AudioManager.instance.PlayAmbient("LowSanityNight");
            else
                AudioManager.instance.PlayAmbient("LowSanityDay");
        }
        else
        {   
            if (IsNight())
                AudioManager.instance.PlayAmbient("ForestNight");
            else
                AudioManager.instance.PlayAmbient("ForestDay");
        }

        wasNight = IsNight();
        wasLowSanity = PlayerController.instance.GetPlayerAttributes().LowSanity();
    }
}
