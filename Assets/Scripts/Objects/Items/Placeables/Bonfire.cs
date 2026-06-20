using Unity.VisualScripting;
using UnityEngine;

public class Bonfire : PlaceableBehaviour, IInteractiveObject
{
    [SerializeField] GameObject fire;
    [SerializeField] GameObject pointLight;
    [SerializeField] private bool fireActive = false;
    private float burnRate = 1f;
    private float burnCooldown = 25f;
    private float fireTimer = 0f;

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