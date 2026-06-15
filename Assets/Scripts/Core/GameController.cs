using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    private bool radioUsed = false;
    private float rescueDays = 14;
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

    public void RadioUsed(bool value) => radioUsed = value;
    public void SetCallDay(int day) => callDay = day;

    void Update()
    {
        if (radioUsed)
        {
            int currentDay = DayCycleController.instance.GetCurrentDay();
            float currentHour = DayCycleController.instance.GetCurrentHour();

            if(currentDay >= callDay + rescueDays)
            {
                if(currentHour > rescueHour)
                {
                    
                }
            }
        }
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOverScene");
    }

}