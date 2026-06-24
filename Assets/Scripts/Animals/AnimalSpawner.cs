using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalSpawner : MonoBehaviour
{   
    public static AnimalSpawner instance;

    [SerializeField] private Transform player;
    [SerializeField] private List<GameObject> animals;
    [SerializeField] private Transform animalsParent;

    private float minSpawnDistance = 120f;
    private float maxSpawnDistance = 180f;
    private float spawnRate = 20f;
    private int maxAnimals = 10;
    private float timer;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate && GetAliveAnimals() < maxAnimals)
        {
            SpawnAnimal();
            timer = 0f;
        }
    }

    private int GetAliveAnimals()
    {
        int count = 0;

        foreach (Transform child in animalsParent)
        {
            Animal animal = child.GetComponent<Animal>();

            if (animal != null && !animal.IsDead())
            {
                count++;
            }
        }

        return count;
    }

    private void SpawnAnimal()
    {
        int index = Random.Range(0, animals.Count);
        GameObject prefab = animals[index];

        Vector3 spawnPos = GetSpawnPosition();

        GameObject animal = Instantiate(prefab, spawnPos, Quaternion.identity);

        NavMeshAgent agent = animal.GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            NavMeshHit hit;

            if (NavMesh.SamplePosition(spawnPos, out hit, 2f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            else
            {
                Debug.LogWarning("Spawn inválido para agent");
                Destroy(animal);
                return;
            }
        }
        
        if(prefab.GetComponent<Animal>().GetAnimalData().alwaysHostile)
        {   
            animal.GetComponent<Animal>().SetBehaviour(new HostileBehaviour());
        }
        else
        {
            if(PlayerController.instance.GetPlayerAttributes().GetCurrentSanity() < PlayerController.instance.GetPlayerAttributes().GetMaxSanity() *0.5f)
            {
                animal.GetComponent<Animal>().SetBehaviour(new HostileBehaviour());
            }
            else
            {   
                animal.GetComponent<Animal>().SetBehaviour(new PacificBehaviour());
            }
        }
        animal.GetComponent<Animal>().SetPlayer(player);
        animal.transform.SetParent(animalsParent);
    }

    private Vector3 GetSpawnPosition()
    {
        Camera cam = Camera.main;

        if(PlayerController.instance.GetPlayerAttributes().GetCurrentSanity() < PlayerController.instance.GetPlayerAttributes().GetMaxSanity() * 0.5f)
        {
            maxAnimals = 12;
            minSpawnDistance = 50;
            maxSpawnDistance = 100;
        }
        else
        {
            maxAnimals = 10;
            minSpawnDistance = 120;
            maxSpawnDistance = 180;
        }
        for(int i = 0; i < 20; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized;

            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);

            Vector3 direction = new Vector3(randomCircle.x, 0, randomCircle.y);

            Vector3 rawPosition = player.position + direction * distance;
            rawPosition.x = Mathf.Clamp(rawPosition.x, -850f, 850f);
            rawPosition.z = Mathf.Clamp(rawPosition.z, -850f, 850f);

            Vector3 dirToSpawn = (rawPosition - cam.transform.position).normalized;

            float dot = Vector3.Dot(cam.transform.forward, dirToSpawn);

            bool isVisible = dot > 0.4f;

            if(isVisible)
                continue;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(rawPosition, out hit, 10f, NavMesh.AllAreas))
                return hit.position;
        }

        return Vector3.zero;
    }
}