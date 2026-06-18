using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class Animal : MonoBehaviour
{
    [SerializeField] protected AnimalData data;
    [SerializeField] private GameObject dropItem;
    [SerializeField] private Material pacificMaterial;
    [SerializeField] private Material hostileMaterial;
    private IAnimalBehaviour animalBehaviour;
    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;


    private MeshCollider meshCollider;
    private SkinnedMeshRenderer skinnedMesh;
    private Mesh bakedMesh;

    private bool dead = false;
    private bool isHostile = false;
    private bool wasHostile = false;
    private bool isFlinching = false;
    private bool isFleeing = false;
    private bool isAttacking = false;

    private float currentHealth;

    void Start()
    {
        currentHealth = data.maxHealth;

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();
        meshCollider = GetComponentInChildren<MeshCollider>();

        bakedMesh = new Mesh();
    }

    public AnimalData GetAnimalData() => data;
    public Transform GetPlayer() => player;
    public void SetPlayer(Transform p) => player = p;
    public void SetBehaviour(IAnimalBehaviour behaviour) => animalBehaviour = behaviour;
    public Animator GetAnimator() => animator;
    public NavMeshAgent GetAgent() => agent;
    public void SetFleeing(bool value) => isFleeing = value;
    public bool IsFleeing() => isFleeing;
    public void SetFlinching(bool value) => isFlinching = value;
    public bool IsFlinching() => isFlinching;
    public void SetAttacking(bool value) => isAttacking = value;
    public bool IsAttacking() => isAttacking;
    public bool IsBusy() => isFlinching || dead ||isAttacking;
    
    void Update()
    {
        isHostile = PlayerController.instance.GetPlayerAttributes().GetCurrentSanity() < PlayerController.instance.GetPlayerAttributes().GetMaxSanity() * 0.5f;
        skinnedMesh.material = isHostile ? hostileMaterial : pacificMaterial; 
        
        if (!dead)
        {   
            /*
            if(isHostile && !wasHostile)
                animalBehaviour = new HostileBehaviour();
            else if(!isHostile && wasHostile)
                animalBehaviour = new PacificBehaviour();

            */
            animalBehaviour?.Act(this);
        }

        wasHostile = isHostile;
        
    }

    public void TakeDamage(float amount)
    {   
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, data.maxHealth);
        
        if (currentHealth <= 0)
        {   
            if(!dead)
                Die();
            else
            {
                DropItems();
                Destroy(gameObject);
            }
        }
        else
        {
            if (!dead)
            {
                animator.SetTrigger("Flinch");
                StartCoroutine(FlinchCR());
            }

            animalBehaviour.TakeDamage(this);
        }
    }

    private void DropItems()
    {
        int amount = Random.Range(2, 5);

        for (int i = 0; i < amount; i++)
        {
            Vector3 offset = transform.position + new Vector3(
                Random.Range(0.2f, 0.5f),
                0.2f,
                Random.Range(-0.2f, 0.2f)
            );

            GameObject item = Instantiate(dropItem, offset, Quaternion.identity);

            Rigidbody rb = item.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 force = (transform.right * Random.Range(1f, 2f)) + 
                                (Vector3.up * Random.Range(-1f, -2f)) +
                                (transform.forward * Random.Range(-0.2f, 0.2f));

                rb.AddForce(force, ForceMode.Impulse);
            }
        }
    }

    public IEnumerator KnockbackCR(Vector3 dir, float force, float duration)
    {
        NavMeshAgent agent = GetAgent();

        agent.isStopped = true;
        agent.updatePosition = true;

        Vector3 start = transform.position;
        Vector3 target = start + dir * force;

        float t = 0f;

        while (t < duration)
        {
            float step = t / duration;

            Vector3 pos = Vector3.Lerp(start, target, step);

            agent.Move(pos - transform.position);

            t += Time.deltaTime;

            yield return null;
        }

        agent.Warp(target);

        agent.isStopped = false;
    }

    private void Die()
    {
        dead = true;
        agent.enabled = false;
        animator.SetTrigger("Dead");

        player.GetComponent<PlayerController>().GetPlayerAttributes().UpdateSanity(-10f);

        currentHealth = data.maxHealth/2;
        StartCoroutine(BakeDeadMeshCR());
    }

    private IEnumerator BakeDeadMeshCR()
    {
        yield return new WaitForSeconds(4f);

        skinnedMesh.BakeMesh(bakedMesh);

        float scale = 0.25f;
        Vector3[] vertices = bakedMesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
            vertices[i] *= scale;
        

        bakedMesh.vertices = vertices;

        bakedMesh.RecalculateBounds();
        bakedMesh.RecalculateNormals();

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = bakedMesh;

        meshCollider.enabled = true;

        animator.enabled = false;
    }

    private IEnumerator FlinchCR()
    {   
        isFlinching = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        yield return new WaitForSeconds(2f);
        isFlinching = false;
        agent.velocity = Vector3.zero;
        agent.isStopped = false;
    }

    public void Attack()
    {
        StartCoroutine(AttackCR());
    }

    public void AttackPlayer()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= data.attackDistance/1.2)
        {
            player.GetComponent<PlayerController>()
                .GetPlayerAttributes()
                .TakeDamage(data.damage);
        }
    }
    public IEnumerator AttackCR()
    {
        isAttacking = true;
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        agent.ResetPath();
        yield return new WaitForSeconds(1f);
        AttackPlayer();
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        agent.isStopped = false;

    }
}