using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameByHover : MonoBehaviour
{
    public float hoverTime = 2f;
    private float timer = 0f;
    private bool hovering = false;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Start");
        if (other.CompareTag("Player"))
        {
            hovering = true;
            Debug.Log("Start");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hovering = false;
            timer = 0f;
        }
    }

    void Update()
    {
        if (hovering)
        {
            timer += Time.deltaTime;
            if (timer >= hoverTime)
            {
                Debug.Log("StartGame");
                GameManager.instance.StartGame();
                hovering = false;
                timer = 0f;
            }
        }
    }
}