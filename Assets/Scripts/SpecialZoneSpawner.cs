using UnityEngine;

public class SpecialZoneSpawner : MonoBehaviour
{
    [Header("Prefabs de zonas especiales")]
    public GameObject[] zonePrefabs;

    [Header("Área donde spawnear")]
    public float minX = -40f, maxX = 40f;
    public float minZ = -20f, maxZ = 20f;
    public float spawnY = 0f;

    [Header("Configuración")]
    public float spawnInterval = 15f;   // Tiempo entre cada aparición
    public float zoneLifetime = 10f;    // Cuánto dura cada zona

    public Transform generatedElements;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnZone), 3f, spawnInterval);
    }

    private void SpawnZone()
    {
        if (zonePrefabs.Length == 0) return;

        const int maxAttempts = 10;
        const float minDistance = 8f; // distancia mínima entre zonas
        const float minDistanceFromPuck = 3f; // distancia mínima del puck

        // Tags que identifican zonas especiales activas
        string[] zoneTags = { "RepulsorZone", "SpeedZone", "TeledirigidaZone" };

        GameObject puck = GameObject.FindGameObjectWithTag("Puck");

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float x = Random.Range(minX, maxX);
            float z = Random.Range(minZ, maxZ);
            Vector3 spawnPos = new Vector3(x, spawnY, z);

            bool tooClose = false;

            // Comprobación de cercanía a zonas existentes
            foreach (string tag in zoneTags)
            {
                foreach (GameObject existing in GameObject.FindGameObjectsWithTag(tag))
                {
                    if (Vector3.Distance(existing.transform.position, spawnPos) < minDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }
                if (tooClose) break;
            }

            // Comprobación de cercanía al puck
            if (!tooClose && puck != null)
            {
                if (Vector3.Distance(puck.transform.position, spawnPos) < minDistanceFromPuck)
                {
                    tooClose = true;
                }
            }

            if (!tooClose)
            {
                int index = Random.Range(0, zonePrefabs.Length);
                GameObject prefab = zonePrefabs[index];

                GameObject zone = Instantiate(prefab, spawnPos, Quaternion.identity, generatedElements);
                Destroy(zone, zoneLifetime);

                Debug.Log($"Zona instanciada: {prefab.name} en {spawnPos}");
                return;
            }
        }

        Debug.LogWarning("No se encontró una posición válida para una nueva zona.");
    }


}
