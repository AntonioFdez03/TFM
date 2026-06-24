using UnityEngine;

public class RescueRope : MonoBehaviour, IInteractiveObject
{
    private float maxLength = 660f;
    private float dropSpeed = 50f;
    private float currentLength;

    void Update()
    {
        currentLength = Mathf.MoveTowards(
            currentLength,
            maxLength,
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

    public void RemoveRope()
    {   
        if(currentLength == 0)
        {
            print("Leaving");
            transform.parent.GetComponent<Helicopter>().Leave();
            return;
        }

        currentLength = Mathf.MoveTowards(
            currentLength,
            0f,
            dropSpeed * Time.deltaTime
        );

        transform.localScale = new Vector3(
            transform.localScale.x,
            currentLength,
            transform.localScale.z
        );
    }
}