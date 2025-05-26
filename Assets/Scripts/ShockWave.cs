using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    public float duration = 1.5f;
    public float maxScale = 15f;
    private float elapsed = 0f;
    private Material mat;
    private Color originalColor;

    public void SetColor(Color c)
    {
        originalColor = c;
        Debug.Log(originalColor);
    }

    void Start()
    {
        Transform wave = transform.GetChild(0);
        mat = wave.GetComponent<Renderer>().material;
        // originalColor = mat.color;
        mat.color = originalColor;
        wave.localScale = Vector3.zero;
    }

    void Update()
    {
        Debug.Log("Scale: " + transform.GetChild(0).localScale);

        elapsed += Time.deltaTime;
        float t = elapsed / duration;

        // Expansión
        float scale = Mathf.Lerp(0f, maxScale, t);
        transform.GetChild(0).localScale = new Vector3(scale, 1f, scale);

        // Desaparición
        Color newColor = originalColor;
        newColor.a = Mathf.Lerp(originalColor.a, 0f, t);
        mat.color = newColor;

        if (t >= 1f)
            Destroy(gameObject);
    }
}
