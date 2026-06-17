using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class HostileBehaviour : IAnimalBehaviour
{   
    private Transform player;
    private NavMeshAgent agent;
    private float attackTimer;
    private float attackDistance = 10;
    private float smoothedSpeed;

    public void Act(Animal animal)
    {   
        attackTimer -= Time.deltaTime;

        agent = animal.GetAgent();
        player = animal.GetPlayer();

        if (player == null || !agent.isOnNavMesh)
            return;

        float distanceToPlayer = Vector3.Distance(animal.transform.position, player.position);

        bool canReach = CanReachTarget(agent, player.position);

        if (distanceToPlayer <= animal.GetAnimalData().chaseDistance)
        {
            if (canReach)
            {
                if (distanceToPlayer > attackDistance)
                {
                    ChasePlayer(animal);
                }
                else if (attackTimer < 0f)
                {   
                    agent.ResetPath();
                    LookAt(animal, player);
                    animal.GetAnimator().SetTrigger("Attack");
                    AttackPlayer(animal);
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
            agent.ResetPath();
        }

        UpdateAnimator(animal);
    }

    void ChasePlayer(Animal animal)
    {
        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh)
            return;

        agent.speed = animal.GetAnimalData().speed * 1.5f;
        agent.SetDestination(player.position);
    }

    private void AttackPlayer(Animal animal)
    {
        if (player != null)
        {
            player.GetComponent<PlayerController>()
                .GetPlayerAttributes()
                .TakeDamage(animal.GetAnimalData().damage);

            attackTimer = animal.GetAnimalData().attackCooldown;
        }
    }

    private void TryAttackStructure(Animal animal)
    {
        Vector3 origin =
            animal.transform.position + Vector3.up * 0.8f;

        Vector3 direction =
            animal.transform.forward;

        if (Physics.SphereCast(
            origin,
            0.7f,
            direction,
            out RaycastHit hit,
            attackDistance))
        {
            if (hit.collider.TryGetComponent(out PlaceableBehaviour placeableBehaviour))
            {   
                LookAt(animal,hit.collider.transform);
                placeableBehaviour.TakeDamage(animal.GetAnimalData().damage);
                attackTimer = animal.GetAnimalData().attackCooldown;
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
                    Time.deltaTime * 10f
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
    }
}