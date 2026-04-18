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
            itemStack = new ItemStack
            {
                id = data.id,
                currentHealth = data.maxHealth
            };
        }
    }

    public virtual void Initialize(ItemStack stack)
    {
        itemStack = stack;

        if (data != null && itemStack == null)
        {
            itemStack = new ItemStack
            {
                id = data.id,
                currentHealth = data.maxHealth
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

    public float GetMaxHealth()
        => data != null ? data.maxHealth : 0;

    public abstract void Use();

    protected IEnumerator UseCooldown()
    {
        yield return new WaitForSeconds(useCooldown);
        canUse = true;
    }

    public virtual void Attack(ArmController arm) { }
}