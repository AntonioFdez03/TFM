using UnityEngine;
using UnityEngine.XR;

[CreateAssetMenu(menuName = "Items/Item Data")]
public class ItemData : ScriptableObject
{
    public string id;
    public string itemName;
    public Sprite icon;
    public GameObject prefab;
    public bool showInHand;
    public HandForm handForm;
    public int maxStack;
}