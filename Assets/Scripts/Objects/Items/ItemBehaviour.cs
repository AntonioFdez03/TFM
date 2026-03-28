using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ItemBehaviour : MonoBehaviour, IObjectHealth
{  
    protected ItemData itemData;
    protected float currentHealth;
    protected float maxHealth = 0;
    protected float useCooldown = 1f;
    protected bool canUse = true;

    protected virtual void Awake()
    {
        itemData = GetComponent<ItemData>();
    }

    public ItemData GetItemData() => itemData;
    public float GetCurrentHealth() => currentHealth;
    public void SetCurrentHealth(float health) => currentHealth = health;
    public float GetMaxHealth() => maxHealth;

    public abstract void Use();

    protected IEnumerator UseCooldown()
    {
        yield return new WaitForSeconds(useCooldown);
        canUse = true;
    }

    public virtual void Attack(ArmController arm){}
}
