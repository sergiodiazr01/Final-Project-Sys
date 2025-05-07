using UnityEngine;

public class FloatingPowerUp : MonoBehaviour
{
    public float amplitude = 0.25f;
    public float frequency = 1f;
    public float rotationSpeed = 45f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float offsetY = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = startPos + new Vector3(0f, offsetY, 0f);
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}