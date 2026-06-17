using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Interactions;
public class PacificBehaviour : IAnimalBehaviour
{
    private float timer;
    private float smoothedSpeed;
    private bool playerDetected;

    private NavMeshAgent agent;


    public void Act(Animal animal)
    {      
        if (animal.IsBusy())
            return;

        Transform player = animal.GetPlayer();
        agent = animal.GetAgent();
        
        float distanceToPlayer = Vector3.Distance(animal.transform.position, player.position);

        Vector3 directionToPlayer = (player.position - animal.transform.position).normalized;
        float dot = Vector3.Dot(animal.transform.forward, directionToPlayer);
        bool isLookingAtPlayer = dot > 0.7f;

        playerDetected = distanceToPlayer <= animal.GetAnimalData().chaseDistance && (!player.GetComponent<PlayerController>().IsCrouching() || isLookingAtPlayer);
        if(playerDetected)
            Flee(animal);
        else
        {   
            animal.SetFleeing(false);
            timer -= Time.deltaTime;
            if (timer <= 0)
                ChooseNewAction(animal);
        }

        UpdateAnimator(animal,agent);
    }

    void ChooseNewAction(Animal animal)
    {   
        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh)
            return;
    
        timer = Random.Range(5f, 20f);

        int action = Random.Range(0, 2);
        agent.isStopped = false;

        if (action == 0)
            agent.ResetPath();
        else
        {
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
        if(!animal.IsFleeing() && !playerDetected) return;

        Transform player = animal.GetPlayer();

        animal.SetFleeing(true);

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


    private void UpdateAnimator(Animal animal, NavMeshAgent agent)
    {
        float rawSpeed = (agent.velocity.magnitude + agent.desiredVelocity.magnitude) / 2;

        if (rawSpeed < 0.05f)
            rawSpeed = agent.desiredVelocity.magnitude;

        smoothedSpeed = Mathf.Lerp(smoothedSpeed, rawSpeed, Time.deltaTime * 10f);

        float maxWalk = animal.GetAnimalData().speed;
        float maxRun = maxWalk * 3f;

        float normalized = Mathf.Clamp01(smoothedSpeed / maxRun);

        float vert = Mathf.InverseLerp(0f, maxWalk, smoothedSpeed);

        animal.GetAnimator().SetFloat("Vert", vert);
        animal.GetAnimator().SetFloat("State", normalized);
    }
}