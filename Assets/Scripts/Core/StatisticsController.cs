using TMPro;
using UnityEngine;

public class StatisticsController : MonoBehaviour
{
    public static StatisticsController instance;

    private int daysSurvived = 0;
    private int treesChopped = 0;
    private int rocksDestroyed = 0;
    private int bushesRecolected = 0;
    private int animalsHunted = 0;
    private int itemsCrafted = 0;
    private int itemsPlaced = 0;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddDaySurvived() => daysSurvived ++;
    public void AddTreeChopped() => treesChopped ++;
    public void AddAnimalHunted() => animalsHunted ++;
    public void AddRockDestroyed() => rocksDestroyed ++;
    public void AddBushRecolected() => bushesRecolected ++;
    public void AddItemCrafted() => itemsCrafted ++;
    public void AddItemPlaced() => itemsPlaced ++;

    public void SetDaysSurvived(int days) => daysSurvived = days;
    public void SetTreesChopped(int trees) => treesChopped = trees;
    public void SetRocksDestroyed(int rocks) => rocksDestroyed = rocks;
    public void SetBushesHarvested(int bushes) => bushesRecolected = bushes;
    public void SetAnimalsHunted(int animals) => animalsHunted = animals;
    public void SetItemsCrafted(int items) => itemsCrafted = items;
    public void SetItemsPlaced(int items) => itemsPlaced = items;

    public int GetDaysSurvived() => daysSurvived;
    public int GetTreesChopped() => treesChopped;
    public int GetRocksDestroyed() => rocksDestroyed;
    public int GetBushesHarvested() => bushesRecolected;
    public int GetAnimalsHunted() => animalsHunted;
    public int GetItemsCrafted() => itemsCrafted;
    public int GetItemsPlaced() => itemsPlaced;

    public string GetStats()
    {
        string stats = "DAYS SURVIVED: " + daysSurvived;

        stats += "\nTREES CHOPPED: " + treesChopped;
        stats += "\nROCKS DESTROYED: " + rocksDestroyed;
        stats += "\nBUSHES HARVESTED: " + bushesRecolected;
        stats += "\nANIMALS HUNTED: " + animalsHunted;
        stats += "\nITEMS CRAFTED: " + itemsCrafted;
        stats += "\nITEMS PLACED: " + itemsPlaced;

        return stats;
    }
    
}