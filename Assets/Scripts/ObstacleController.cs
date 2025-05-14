using UnityEngine;
using System.Collections;

public class ObstacleController : MonoBehaviour
{
    [Header("Timing (segundos)")]
    public float riseTime = 1f;   // tiempo de subir
    public float activeTime = 10f;  // tiempo visible
    public float fallTime = 1f;   // tiempo de descender

    [Header("Alturas (metros)")]
    public float hiddenDepth = 1f;  // cuánto está bajo tierra al inicio
    public float visibleHeight = 0.5f; // cuánto sobresale al final

    void Start()
    {
        StartCoroutine(Lifecycle());
    }

    IEnumerator Lifecycle()
    {
        // 1) posición inicial escondida
        Vector3 startPos = transform.position + Vector3.down * hiddenDepth;
        // 2) posición final sobresaliendo
        Vector3 endPos = transform.position + Vector3.up * visibleHeight;
        transform.position = startPos;

        float t = 0f;
        // Subir
        while (t < riseTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t / riseTime);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;

        // Permanecer
        yield return new WaitForSeconds(activeTime);

        // Bajar
        t = 0f;
        while (t < fallTime)
        {
            transform.position = Vector3.Lerp(endPos, startPos, t / fallTime);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos;

        // Destruir
        Destroy(gameObject);
    }
}
