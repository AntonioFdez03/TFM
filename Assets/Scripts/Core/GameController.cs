using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [SerializeField] private GameObject helicopterPrefab;
    [SerializeField] private Transform helicopterSpawn;
    [SerializeField] private Transform helicopterTarget;
    private bool helicopterInstantiated = false;

    private bool radioUsed = false;
    private int daysUntilRescue = 3;
    private float rescueHour = 0;

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
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOverScene");
    }

}