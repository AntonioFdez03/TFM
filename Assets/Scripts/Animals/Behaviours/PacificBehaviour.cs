using UnityEngine;

public class PacificBehaviour : IAnimalBehaviour
{

    public void Act(Animal animal)
    {
        Debug.Log(animal.name + " no ataca");
    }
}