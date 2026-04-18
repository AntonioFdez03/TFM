using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public abstract class ConsumableBehaviour : ItemBehaviour
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
            print("Entra");
            timer = 0;
            PlayerAttributes player = PlayerController.instance.GetPlayerAttributes();
            if(player.GetCurrentHunger() < player.GetMaxHunger())
                Consume();
        }
    }

    protected abstract void Consume();

    public float GetCurrentTime() => timer;
    public float SetCurrentTime(float time) => timer = time;
    public float GetConsumeTime() => consumableData.consumeTime;
}
