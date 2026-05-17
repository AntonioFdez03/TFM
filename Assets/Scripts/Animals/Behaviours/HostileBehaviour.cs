using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class HostileBehaviour : IAnimalBehaviour
{
    private float attackDistance = 10f;
    private float chaseDistance = 50f;
    private float losePlayerDistance = 100f;

    private float attackCooldown = 2f;
    private float attackTimer;
    private float damage = 2f;

    public void Act(Animal animal)
    {   
        attackTimer -= Time.deltaTime;

        NavMeshAgent agent = animal.GetAgent();
        Transform player = animal.GetPlayer();

        if (player == null || !agent.isOnNavMesh)
            return;

        float distanceToPlayer = Vector3.Distance(animal.transform.position, player.position);

        bool canReach = CanReachTarget(agent, player.position);

        if (distanceToPlayer <= chaseDistance)
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
                    LookAtPlayer(animal);
                    animal.GetAnimator().SetTrigger("Attack");
                    AttackPlayer(animal);
                }
            }
            else if(attackTimer < 0f)
            {
                agent.ResetPath();
                LookAtPlayer(animal);
                animal.GetAnimator().SetTrigger("Attack");
                TryAttackStructure(animal);
            }
        }
        else
        {
            agent.ResetPath();
        }

        // Animaciones 
        float speed = agent.velocity.magnitude; 
        animal.GetAnimator().SetFloat("Vert", speed > 0.1f ? 1 : 0); 
        animal.GetAnimator().SetFloat( "State", speed > animal.GetAnimalData().speed * 1.1f ? 1 : 0 );
    }

    void ChasePlayer(Animal animal)
    {
        NavMeshAgent agent = animal.GetAgent();
        Transform player = animal.GetPlayer();

        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh)
            return;

        agent.speed = animal.GetAnimalData().speed * 1.5f;
        agent.SetDestination(player.position);
    }

    private void AttackPlayer(Animal animal)
    {
        Transform player = animal.GetPlayer();

        if (player != null)
        {
            player.GetComponent<PlayerController>()
                .GetPlayerAttributes()
                .TakeDamage(damage);

            attackTimer = attackCooldown;
        }
    }

    private void TryAttackStructure(Animal animal)
    {

        Transform player = animal.GetPlayer();

        Vector3 origin = animal.transform.position + Vector3.up;
        Vector3 dir = (player.position - origin).normalized;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, attackDistance))
        {
            if (hit.collider.TryGetComponent(out PlaceableBehaviour placeableBehaviour))
            {
                placeableBehaviour.TakeDamage(damage);

                attackTimer = attackCooldown;
            }
        }
    }

    public void TakeDamage(Animal animal)
    {
        // Al recibir daño se vuelve aún más agresivo
        ChasePlayer(animal);
    }

    private void LookAtPlayer(Animal animal)
    {
        Transform player = animal.GetPlayer();

        Vector3 direction =
            player.position - animal.transform.position;

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
}