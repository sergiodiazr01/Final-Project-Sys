using UnityEngine;
using UnityEngine.SceneManagement;

public class returnToMenuProx : MonoBehaviour
{
    public float hoverTime = 2f;
    private float timer = 0f;
    private bool isHovering = false;
    public GameManager gameManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isHovering = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isHovering = false;
            timer = 0f;
        }
    }

    void Update()
    {
        if (isHovering)
        {
            timer += Time.deltaTime;
            if (timer >= hoverTime)
            {
                SceneManager.LoadScene("Scene");
                isHovering = false;
                timer = 0f;
            }
        }
    }
}
