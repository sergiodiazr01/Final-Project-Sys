using UnityEngine;
using System.Collections;

public class RockSpawner : MonoBehaviour
{
    [Header("Prefab y tiempos")]
    public GameObject rockPrefab;
    public GameObject indicatorPrefab;   
    public float spawnInterval = 5f;     //tiempo entre spawn (rocai)
    public float initialDelay = 0f;    //retardo inicial
    public float indicatorTime = 1f;    //duracion de la señal antes de la roca

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
        Vector3 c = transform.TransformPoint(areaCollider.center);
        Vector3 h = areaCollider.size * 0.5f;
        float excl = 10f;
        float x = Random.Range(c.x - h.x, c.x + h.x);

        //límites de Z
        float zMin = c.z - h.z;
        float zMax = c.z + h.z;
        float zLow = c.z - excl;
        float zHigh = c.z + excl;

        //longitudes de tramo inferior y superior
        float l1 = Mathf.Max(0, zLow - zMin);
        float l2 = Mathf.Max(0, zMax - zHigh);

        //elige zona en proporción a sus longitudes y genera Z
        float z = Random.value * (l1 + l2) < l1
                       ? Random.Range(zMin, zLow)
                       : Random.Range(zHigh, zMax);

        Vector3 p = new Vector3(x, c.y, z);
        var ind = Instantiate(indicatorPrefab, p, Quaternion.identity, generatedElements);
        yield return new WaitForSeconds(indicatorTime);
        Destroy(ind);
        Instantiate(rockPrefab, p, Quaternion.identity, generatedElements);
    }
}