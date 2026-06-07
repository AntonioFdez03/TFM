using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class Torch : ToolBehaviour, IActivateableObject
{
    [SerializeField] GameObject fire;
    [SerializeField] GameObject pointLight;
    private bool fireActive;
    private float burnRate = 1f;
    private float burnCooldown = 10f;
    private float timer = 0f;

    protected override void Start()
    {
        base.Start();

        fireActive = false;
    }

    public bool isActive() => fireActive;

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

    public void ToggleActivation()
    {
        fireActive = !fireActive;
    }
}