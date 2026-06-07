using Unity.VisualScripting;
using UnityEngine;

public class StandingTorch : PlaceableBehaviour, IInteractiveObject
{
    [SerializeField] GameObject fire;
    [SerializeField] GameObject pointLight;

    private bool fireActive;
    private float burnRate = 1f;
    private float burnCooldown = 30f;
    private float timer = 0f;


    protected override void Start()
    {
        base.Start();

        fireActive = false;
    }

    void Update()
    {   
        fire.SetActive(fireActive);
        pointLight.SetActive(fireActive);

        if (fireActive)
        {   
            timer += Time.deltaTime;

            if(timer >= burnCooldown)
            {
                timer = 0;
                TakeDamage(burnRate);
            }
        }
        else
        {
            timer = 0;
        }
    }

    public void Interact()
    {
        fireActive = !fireActive;
    }
}