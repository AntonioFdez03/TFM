using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class HostileBehaviour : IAnimalBehaviour
{   
    private Transform player;
    private NavMeshAgent agent;
    private float attackTimer;
    private float smoothedSpeed;

    private bool playerDetected = false;
    private float timer;

    public void Act(Animal animal)
    {   
        if(animal.IsAttacking())
            LookAt(animal, player);

        if (animal.IsBusy())
            return;

        attackTimer -= Time.deltaTime;

        agent = animal.GetAgent();
        player = animal.GetPlayer();

        if (player == null || !agent.isOnNavMesh)
            return;

        float distanceToPlayer = Vector3.Distance(animal.transform.position, player.position);

        bool canReach = CanReachTarget(agent, player.position);
        agent.isStopped = false;

        if (distanceToPlayer <= animal.GetAnimalData().chaseDistance)
        {
            if (!playerDetected)
            {
                playerDetected = true;
                animal.DetectSound();
            }
            if (canReach)
            {
                if (distanceToPlayer > animal.GetAnimalData().attackDistance + 0.5f)
                {   
                    ChasePlayer(animal);
                }
                else if (attackTimer < 0f)
                {   
                    animal.Attack("Player");
                    LookAt(animal, player);
                    animal.GetAnimator().SetTrigger("Attack");
                    attackTimer = animal.GetAnimalData().attackCooldown;
                }
            }
            else
            {
                MoveToClosestReachablePoint(animal);

                // Solo atacar cuando ya haya llegado
                if (!agent.pathPending &&
                    agent.remainingDistance <= agent.stoppingDistance &&
                    attackTimer < 0f)
                    {
                        agent.ResetPath();
                        //LookAtPlayer(animal);
                        animal.GetAnimator().SetTrigger("Attack");
                        attackTimer = animal.GetAnimalData().attackCooldown;
                        TryAttackStructure(animal);
                    }
            }
        }
        else
        {   
            timer -= Time.deltaTime;
            if (timer <= 0)
                ChooseNewAction(animal);
        
        }

        UpdateAnimator(animal);
    }

    void ChasePlayer(Animal animal)
    {
        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh)
            return;

        agent.speed = animal.GetAnimalData().speed * 1.5f;

        Vector3 dir = (animal.transform.position - player.position).normalized;
        Vector3 targetPos = player.position + dir * animal.GetAnimalData().attackDistance;

        agent.SetDestination(targetPos);
    }

    private void TryAttackStructure(Animal animal)
    {
        Collider[] hits = Physics.OverlapSphere(
            animal.transform.position,
            animal.GetAnimalData().attackDistance);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out PlaceableBehaviour placeableBehaviour))
            {
                Vector3 direction =
                    hit.transform.position - animal.transform.position;

                direction.y = 0;

                if (direction != Vector3.zero)
                {
                    animal.transform.rotation =
                        Quaternion.LookRotation(direction);
                }

                animal.Attack("Placeable", placeableBehaviour);

                return;
            }
        }
    }

    public void TakeDamage(Animal animal)
    {
        ChasePlayer(animal);
    }

    private void LookAt(Animal animal, Transform target)
    {
        Vector3 direction = target.position - animal.transform.position;

        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion rotation =
                Quaternion.LookRotation(direction);

            animal.transform.rotation =
                Quaternion.Slerp(
                    animal.transform.rotation,
                    rotation,
                    Time.deltaTime * 20f
                );
        }
    }

    private bool CanReachTarget(NavMeshAgent agent, Vector3 target)
    {
        NavMeshPath path = new NavMeshPath();

        agent.CalculatePath(target, path);

        return path.status == NavMeshPathStatus.PathComplete;
    }

    private void MoveToClosestReachablePoint(Animal animal)
    {
        NavMeshPath path = new NavMeshPath();

        if (agent.CalculatePath(player.position, path))
        {
            // Si hay algún tramo del camino
            if (path.corners.Length > 0)
            {
                // Último punto alcanzable
                Vector3 closestPoint =
                    path.corners[path.corners.Length - 1];

                agent.SetDestination(closestPoint);
            }
        }
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

    private void UpdateAnimator(Animal animal)
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

        animal.UpdateFootsteps(smoothedSpeed);
    }
}