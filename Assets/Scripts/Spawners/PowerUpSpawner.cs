using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public float minX = -44f, maxX = 44f;
    public float minZ = -24f, maxZ = 24f;
    public float spawnY = 0.5f;

    public GameObject powerUpBoxPrefab;
    public float spawnInterval = 10f; //tiempo entre spawns

    public Transform generatedElements;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnPowerUpBox), 2f, spawnInterval);
    }

    private void SpawnPowerUpBox()
    {
        if (powerUpBoxPrefab == null) return;

        float randomX = Random.Range(minX, maxX);
        float randomZ = Random.Range(minZ, maxZ);
        Vector3 spawnPos = new Vector3(randomX, spawnY, randomZ);

        Instantiate(powerUpBoxPrefab, spawnPos, Quaternion.identity, generatedElements);
        Debug.Log("Se ha spawneado una PowerUp Box en " + spawnPos);
    }
}