using UnityEngine;

public class RescueRope : MonoBehaviour, IInteractiveObject
{
    private float maxLength = 660f;
    private float dropSpeed = 50f;
    private float currentLength;

    void Update()
    {   
        float targetLenght = maxLength;

        if(transform.parent.TryGetComponent(out Helicopter helicopter))
        {   
            if(targetLenght == 0 && currentLength == 0)
            {
                helicopter.Leave();
                return;
            }

            if(helicopter.TimePassed())
                targetLenght = 0;
        }

        currentLength = Mathf.MoveTowards(
            currentLength,
            targetLenght,
            dropSpeed * Time.deltaTime
        );

        transform.localScale = new Vector3(
            transform.localScale.x,
            -currentLength,
            transform.localScale.z
        );
    }

    public void Interact()
    {
        GameController.instance.SetPlayerEscaped(true);
    }
}