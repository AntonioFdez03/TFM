using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    private bool radioUsed = false;
    private int daysUntilRescue = 14;
    private int callDay = 10000;
    private float rescueHour = 7;

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
    public void SetCallDay(int day) => callDay = day;

    void Update()
    {
        if (radioUsed)
        {
            float currentHour = DayCycleController.instance.GetCurrentHour();

            if(DayCycleController.instance.GetObjectiveDaysSurvived() == daysUntilRescue)
            {
                if(currentHour > rescueHour)
                {
                    ObjectivesController.instance.NextObjective(ObjectiveType.Escape);
                }
            }
        }
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOverScene");
    }

}