using UnityEngine;

public class HostileBehaviour : IAnimalBehaviour
{
    public void Act(Animal animal)
    {
        Debug.Log(animal.name + " ataca");
    }

    public void TakeDamage(Animal animal)
    {
        
    }


}