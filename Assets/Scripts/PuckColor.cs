using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckColor : MonoBehaviour
{
    public Renderer puckRenderer;

    public Color defaultColor = Color.white;
    public Color redPlayerColor = Color.red;
    public Color bluePlayerColor = Color.blue;

    void Start()
    {
        puckRenderer.material.color = defaultColor;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerRed"))
        {
            Debug.Log("a");
            puckRenderer.material.color = redPlayerColor;
        }
        else if (collision.gameObject.CompareTag("PlayerBlue"))
        {
            puckRenderer.material.color = bluePlayerColor;
        }
    }
}
