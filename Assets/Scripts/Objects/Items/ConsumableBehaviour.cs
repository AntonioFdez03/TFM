using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class ConsumableBehaviour : ItemBehaviour
{   
    protected ConsumableData consumableData;
    protected float timer;

    InputAction attack;

    public override void Initialize(ItemStack stack)
    {
        base.Initialize(stack);
        consumableData = data as ConsumableData;
    }

    public override void Use()
    {   
        attack = InputSystem.actions.FindAction("Attack");

        if (attack.IsPressed())
            timer += Time.deltaTime;

        if(timer > consumableData.consumeTime)
        {
            timer = 0;
            PlayerAttributes player = PlayerController.instance.GetPlayerAttributes();
            if(player.GetCurrentHunger() < player.GetMaxHunger())
                Consume();
        }
    }

    protected void Consume()
    {   
        if(consumableData.hungerPoints > 0)
            PlayerController.instance.GetPlayerAttributes().Eat(consumableData.hungerPoints);
        
        if(consumableData.healthPoints != 0)
            PlayerController.instance.GetPlayerAttributes().UpdateHealth(consumableData.healthPoints);

        if(consumableData.sanityPoints != 0)
            PlayerController.instance.GetPlayerAttributes().UpdateSanity(consumableData.sanityPoints);

        InventoryController.instance.RemoveItem(HotBarController.instance.GetCurrentItem());
    }

    public float GetCurrentTime() => timer;
    public float SetCurrentTime(float time) => timer = time;
    public float GetConsumeTime() => consumableData.consumeTime;
}
