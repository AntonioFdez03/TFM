using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalSpawner : MonoBehaviour
{   
    [SerializeField] private Transform player;
    [SerializeField] private List<GameObject> animals;
    [SerializeField] private Transform animalsParent;

    private float minSpawnDistance = 10f;
    private float maxSpawnDistance = 20f;
    private float spawnRate = 20f;
    private int maxAnimals = 1;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate && animalsParent.childCount < maxAnimals)
        {
            SpawnAnimal();
            timer = 0f;
        }
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

        if(PlayerController.instance.GetPlayerAttributes().GetCurrentSanity() < PlayerController.instance.GetPlayerAttributes().GetMaxSanity() *0.5f)
        {
            animal.GetComponent<Animal>().SetBehaviour(new HostileBehaviour());
        }
        else
        {
            animal.GetComponent<Animal>().SetBehaviour(new PacificBehaviour());
        }
        animal.GetComponent<Animal>().SetPlayer(player);
        animal.transform.SetParent(animalsParent);
    }

    private Vector3 GetSpawnPosition()
    {
        // 1. Dirección aleatoria alrededor del jugador
        Vector2 randomCircle = Random.insideUnitCircle.normalized;
        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);

        Vector3 direction = new Vector3(randomCircle.x, 0, randomCircle.y);
        Vector3 rawPosition = player.position + direction * distance;

        // 2. Ajustar al NavMesh (esto ya respeta altura del terreno)
        NavMeshHit hit;
        if (NavMesh.SamplePosition(rawPosition, out hit, 10f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // fallback
        return player.position;
    }
}