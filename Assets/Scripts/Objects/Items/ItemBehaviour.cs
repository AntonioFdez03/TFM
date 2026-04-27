using System.Collections;
using UnityEngine;

public abstract class ItemBehaviour : MonoBehaviour, IObjectHealth
{
    [SerializeField] protected ItemData data;

    protected ItemStack itemStack = null;

    protected float useCooldown = 1f;
    protected bool canUse = true;

    void Start()
    {
        if (data != null && itemStack == null)
        {   
            float maxHealth = 0;
            if(data is DurableItemData durableItemData)
                maxHealth = durableItemData.maxHealth;

            itemStack = new ItemStack
            {
                id = data.id,
                currentHealth = maxHealth
            };
        }
    }

    public virtual void Initialize(ItemStack stack)
    {
        itemStack = stack;

        if (data != null && itemStack == null)
        {   
            float maxHealth = 0;
            if(data is DurableItemData durableItemData)
                maxHealth = durableItemData.maxHealth;

            itemStack = new ItemStack
            {
                id = data.id,
                currentHealth = maxHealth
            };
        }
    }

    public ItemData GetData() => data;
    public ItemStack GetItemStack() => itemStack;

    public float GetCurrentHealth() => itemStack != null ? itemStack.currentHealth : 0;

    public void SetCurrentHealth(float health)
    {
        if (itemStack != null)
            itemStack.currentHealth = health;
    }

    public float GetMaxHealth() {

        if(data != null && data is DurableItemData durableItemData)
            return durableItemData.maxHealth;
        
        return 0;
    }

    public abstract void Use();

    protected IEnumerator UseCooldown()
    {
        yield return new WaitForSeconds(useCooldown);
        canUse = true;
    }

    public virtual void Attack(ArmController arm) { }
}