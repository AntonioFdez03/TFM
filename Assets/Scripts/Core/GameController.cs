using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [SerializeField] private GameObject helicopterPrefab;
    [SerializeField] private Transform helicopterSpawn;
    [SerializeField] private Transform helicopterTarget;
    private bool helicopterInstantiated = false;

    private float requiredHeight = 260;
    private bool radioUsed = false;
    private int daysUntilRescue = 0;
    private float rescueHour = 0;
    private float helicoperWaitTime = 120f;
    private bool playerEscaped = false;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public int GetDaysUntilRescue() => daysUntilRescue;
    public void RadioUsed(bool value) => radioUsed = value;
    public float GetRequiredHeight() => requiredHeight;
    public float GetHelicopterWaitTime() => helicoperWaitTime;
    public void SetPlayerEscaped(bool value) => playerEscaped = value;

    void Update()
    {
        if (radioUsed)
        {
            float currentHour = DayCycleController.instance.GetCurrentHour();

            if(DayCycleController.instance.GetObjectiveDaysSurvived() == daysUntilRescue && !helicopterInstantiated)
            {   
                if(currentHour > rescueHour)
                {   
                    helicopterInstantiated = true;
                    ObjectivesController.instance.NextObjective(ObjectiveType.Escape);
                    GameObject helicopter = Instantiate(helicopterPrefab);
                    helicopter.transform.SetParent(InventoryController.instance.GetItemsParent());
                    helicopter.transform.position = helicopterSpawn.position;
                    helicopter.GetComponent<Helicopter>().SetTarget(helicopterTarget.position);
                }
            }
        }

        if (playerEscaped)
        {
            StartCoroutine(FaderCR("GameCompleteScene"));
        }
    }

    public void GameOver()
    {
        StartCoroutine(FaderCR("GameOverScene"));
    }

    private IEnumerator FaderCR(string sceneName)
    {   
        ScreenFaderController.instance.FadeOut();
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(sceneName);
    }

}