using UnityEngine;
using System.Collections;

public class RockSpawner : MonoBehaviour
{
    [Header("Prefab y tiempos")]
    public GameObject rockPrefab;
    public GameObject indicatorPrefab;   // <— nuevo
    public float spawnInterval = 5f;     // tiempo entre spawn (rocai)
    public float initialDelay = 0f;    // retardo inicial
    public float indicatorTime = 1f;    // duración de la señal antes de la roca

    private float timer;
    private BoxCollider areaCollider;

    public Transform generatedElements;

    void Awake()
    {
        areaCollider = GetComponent<BoxCollider>();
    }

    void Start()
    {
        timer = initialDelay;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            StartCoroutine(SpawnWithIndicator());
            timer = spawnInterval;
        }
    }

    IEnumerator SpawnWithIndicator()
    {
        // 1) Calcula posición aleatoria como antes
        Vector3 center = transform.TransformPoint(areaCollider.center);
        Vector3 size = areaCollider.size;
        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);
        Vector3 spawnPos = new Vector3(x, center.y, z);

        // 2) Instancia el indicador en el suelo
        GameObject indicator = Instantiate(indicatorPrefab, spawnPos, Quaternion.identity, generatedElements);

        // 3) Espera el tiempo de la señal
        yield return new WaitForSeconds(indicatorTime);

        // 4) Destruye el indicador y crea la roca
        Destroy(indicator);
        Instantiate(rockPrefab, spawnPos, Quaternion.identity, generatedElements);
    }
}