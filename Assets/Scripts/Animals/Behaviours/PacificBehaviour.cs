using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PacificBehaviour : IAnimalBehaviour
{
    float timer;
    bool isMoving;

    public void Act(Animal animal)
    {
        NavMeshAgent agent = animal.GetAgent();

        timer -= Time.deltaTime;

        if (timer <= 0)
            ChooseNewAction(animal);

        float speed = agent.velocity.magnitude;

        animal.GetAnimator().SetFloat("Vert", speed > 0.1f ? 1 : 0);

        animal.GetAnimator().SetFloat("State",
            agent.velocity.magnitude > animal.GetAnimalData().speed * 1.1f ? 1 : 0
        );
    }

    void ChooseNewAction(Animal animal)
    {   
        NavMeshAgent agent = animal.GetAgent();

        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh)
            return;
    
        timer = Random.Range(5f, 20f);

        int action = Random.Range(0, 2);

        if (action == 0)
        {
            isMoving = false;
            agent.ResetPath();
        }
        else
        {
            isMoving = true;

            Vector3 randomDirection = Random.insideUnitSphere * 40f;
            randomDirection += animal.transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, 40f, NavMesh.AllAreas))
            {
                agent.speed = animal.GetAnimalData().speed;
                agent.SetDestination(hit.position);
            }
        }
    }

    public void TakeDamage(Animal animal)
    {
        Flee(animal);
    }

    void Flee(Animal animal)
    {
        NavMeshAgent agent = animal.GetAgent();
        Transform player = animal.GetPlayer();

        if (!agent.isOnNavMesh || player == null)
            return;

        Vector3 bestDirection = Vector3.zero;
        float bestScore = -Mathf.Infinity;

        Vector3 origin = animal.transform.position;

        // probar varias direcciones
        for (int i = 0; i < 12; i++)
        {
            float angle = Random.Range(0f, 360f);
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

            Vector3 candidate = origin + dir * 100f;

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                float distanceToPlayer = Vector3.Distance(hit.position, player.position);

                if (distanceToPlayer > bestScore)
                {
                    bestScore = distanceToPlayer;
                    bestDirection = hit.position;
                }
            }
        }

        if (bestScore > 0)
        {
            agent.ResetPath();
            agent.SetDestination(bestDirection);
            agent.speed = animal.GetAnimalData().speed * 3;
        }
    }
}