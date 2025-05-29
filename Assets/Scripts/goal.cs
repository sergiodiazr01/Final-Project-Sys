using System.Collections;
using UnityEngine;

public class goal : MonoBehaviour
{
    public PlayerTeam goalTeam;                // El equipo que recibe el gol si entra aquí
    public GameObject puckPrefab;              // Prefab del puck

    private AudioSource audioSource;           // Componente de AudioSource para reproducir sonidos
    public AudioClip goalSound;              // Sonido del gol 
    
    private void Start()
    {
        // Obtener el componente AudioSource del GameObject
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Puck"))
        {
            // Reproducir sonido de gol
            if (goalSound != null)
            {
                audioSource.PlayOneShot(goalSound);
            }

            // Determinar quién marca (el rival de la portería)
            PlayerTeam scoringTeam = (goalTeam == PlayerTeam.Red) ? PlayerTeam.Blue : PlayerTeam.Red;
            GameManager.instance.GoalScored(scoringTeam);

            // Destruir el puck actual
            GameObject puckGO = other.GetComponentInParent<PuckColor>()?.gameObject;
            if (puckGO != null)
                Destroy(puckGO);
            else
                Debug.LogWarning("No se pudo encontrar el Puck para destruirlo tras el gol.");

            // Iniciar respawn del nuevo puck
            GameManager.instance.RespawnPuck();

            // Elegir color contrario a la portería
            Color waveColor;
            if (goalTeam == PlayerTeam.Red)
            {
                waveColor = new Color(0f, 0f, 255f / 188f);  // onda azul
            }
            else
            {
                waveColor = new Color(255f / 188f, 0f, 0f); // onda roja
            }
            GameManager.instance.TriggerShockwave(transform.position + new Vector3(0f, 8f, 0f), waveColor);

        }
    }

}

