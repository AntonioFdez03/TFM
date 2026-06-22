using System.Collections;
using TMPro;
using UnityEngine;

public enum ObjectiveType { None, Explore, RadioSignal, SendSOS, Survive, Escape}
public class ObjectivesController: MonoBehaviour
{
    public static ObjectivesController instance;

    [SerializeField] private TMP_Text objectiveText;
    private ObjectiveType currentObjective;
    private string currentText = "";
    private int daysToSurvive = 14;

    private void Awake()
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
        SetObjective(ObjectiveType.Explore);
    }

    public ObjectiveType GetCurrentObjective() => currentObjective;

    public void SetObjective(ObjectiveType objective)
    {
        currentObjective = objective;

        switch(objective)
        {
            case ObjectiveType.Explore:
                currentText = "Find a way to escape.";
                break;

            case ObjectiveType.RadioSignal:
                currentText = "Find a high place.";
                break;

            case ObjectiveType.SendSOS:
                currentText = "Send a SOS signal.";
                break;

            case ObjectiveType.Survive:
                currentText = "Survive " +  daysToSurvive + " days more.\n(0/14)";
                break;

            case ObjectiveType.Escape:
                currentText = "Escape in the helicopter.";
                break;

            case ObjectiveType.None:
                currentText = "";
                break;
        }

        objectiveText.text = currentText;
    }

    public void UpdateSurviveObjective(int daysSurvived)
    {
        if(currentObjective == ObjectiveType.Survive)
        {   
            int daysToSurvive = GameController.instance.GetDaysUntilRescue();
            currentText = "Survive " +  daysToSurvive + "days more.\n(" + daysSurvived +"/" + daysToSurvive + ")";
        }
    }
    public void NextObjective(ObjectiveType nextObjective)
    {
        StartCoroutine(CompleteObjectiveCR(nextObjective));
    }

    private IEnumerator CompleteObjectiveCR(ObjectiveType nextObjective)
    {
        objectiveText.text = $"<color=green><s>{currentText}</s></color>";

        yield return new WaitForSeconds(2f);

        SetObjective(nextObjective);
    }
}