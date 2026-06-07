using Unity.VisualScripting;
using UnityEngine;

public class StandingTorch : PlaceableBehaviour, IInteractiveObject
{
    [SerializeField] GameObject fire;
    [SerializeField] GameObject pointLight;

    private bool fireActive;
    private float burnRate = 1f;
    private float burnCooldown = 30f;
    private float fireTimer = 0f;


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
            fireTimer += Time.deltaTime;

            if(fireTimer >= burnCooldown)
            {
                fireTimer = 0;
                TakeDamage(burnRate);
            }
        }
        else
        {
            fireTimer = 0;
        }
    }

    public void Interact()
    {
        fireActive = !fireActive;
    }
}