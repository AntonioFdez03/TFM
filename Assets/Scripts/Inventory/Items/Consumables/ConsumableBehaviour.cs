using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public abstract class ConsumableBehaviour : ItemBehaviour
{   
    protected float consumeTime;
    protected float timer;

    InputAction attack;

    protected override void Awake()
    {
        base.Awake();
        attack = InputSystem.actions.FindAction("Attack");
    }

    public override void Use()
    {
        if (attack.IsPressed())
        {
            print("Pulsando tecla");
            timer += Time.deltaTime;
        }

        if(timer > consumeTime)
        {
            timer = 0;
            Consume();
        }  
    }

    protected abstract void Consume();

    public float GetCurrentTime() => timer;
    public float SetCurrentTime(float time) => timer = time;
    public float GetConsumeTime() => consumeTime;
}
