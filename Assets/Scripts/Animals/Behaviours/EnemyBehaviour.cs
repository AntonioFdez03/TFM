using UnityEngine;

public class EnemyBehaviour : IAnimalBehaviour
{
    public void Act(Animal animal)
    {
        Debug.Log(animal.name + " ataca");
    }
}