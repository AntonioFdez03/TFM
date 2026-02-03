using Unity.VisualScripting;
using UnityEngine;

public abstract class ItemBehaviour : MonoBehaviour
{   
    protected abstract void Awake();
    protected abstract void Use();
}
