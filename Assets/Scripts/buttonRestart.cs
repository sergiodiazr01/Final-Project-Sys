using UnityEngine;

public class buttonRestart : MonoBehaviour
{
    public float activationTime = 2f; // Segundos necesarios para activar
    private float timeInside = 0f;
    private bool isPlayerInside = false;

    public GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            timeInside = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            timeInside = 0f;
        }
    }

    private void Update()
    {
        if (isPlayerInside)
        {
            timeInside += Time.deltaTime;
            if (timeInside >= activationTime)
            {
                gameManager.ReturnToMenu();
                isPlayerInside = false; // evitar múltiples activaciones
            }
        }
    }
}
