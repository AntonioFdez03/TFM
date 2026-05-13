using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    [SerializeField] protected AnimalData data;
    [SerializeField] private GameObject dropItem;
    private IAnimalBehaviour animalBehaviour;
    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;


    private MeshCollider meshCollider;
    private SkinnedMeshRenderer skinnedMesh;
    private Mesh bakedMesh;

    private bool dead = false;

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

    void Update()
    {
        if (!dead)
        {
            animalBehaviour?.Act(this);
        }
    }

    public void TakeDamage(float amount)
    {   
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, data.maxHealth);

        print("Vida restante: " + currentHealth);
        
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
}