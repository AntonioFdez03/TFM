using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class ConsumableBehaviour : ItemBehaviour
{   
    protected ConsumableData consumableData;
    protected float timer;
    protected bool isConsuming;
    protected string consumeSound = "";

    InputAction rmb;

    public override void Initialize(ItemStack stack)
    {
        base.Initialize(stack);
        consumableData = data as ConsumableData;

        if(consumableData.id == "Bandage")
            consumeSound = "Bandage";
        else if(consumableData.hungerPoints > 0)
            consumeSound = "PlayerEat";
    }

    public void SetConsuming(bool value) => isConsuming = value;
    public override void Use()
    {   
        rmb = InputSystem.actions.FindAction("RMB");
        PlayerAttributes player = PlayerController.instance.GetPlayerAttributes();

        bool holding = rmb.IsPressed();

        if (holding && player.CanConsume(consumableData))
        {
            timer += Time.deltaTime;

            if (!isConsuming)
            {
                isConsuming = true;

                if(consumeSound != "")
                    AudioManager.instance.PlayLoop(consumeSound);
            }
        }

        if (timer > consumableData.consumeTime)
        {
            timer = 0;
            AudioManager.instance.StopLoop();
            isConsuming = false;
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
    public void SetCurrentTime(float time) => timer = time;
    public void ResetConsume(){
        timer = 0;
        isConsuming = false;
        AudioManager.instance.StopLoop();
    }
    public float GetConsumeTime() => consumableData.consumeTime;

    public override void Attack(ArmController arm)
    {
        base.Attack(arm);
        StartCoroutine(ArmController.instance.PunchMovementCR());
    }
}
