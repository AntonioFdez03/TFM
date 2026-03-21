using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{   
    [SerializeField] private Transform player;
    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private Transform enemiesParent;

    private float minSpawnDistance = 10f;
    private float maxSpawnDistance = 20f;
    private float spawnRate = 20f;
    private int maxEnemies = 10;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate && GameObject.FindGameObjectsWithTag("Enemy").Length < maxEnemies && DayCycleController.instance.IsNight())
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        int index = Random.Range(0, enemies.Count);
        GameObject prefab = enemies[index];

        Vector3 spawnPos = GetSpawnPosition();

        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        enemy.transform.SetParent(enemiesParent);
        enemy.GetComponent<Enemy>().SetTarget(player);
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