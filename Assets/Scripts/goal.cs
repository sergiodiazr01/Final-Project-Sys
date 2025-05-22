using System.Collections;
using UnityEngine;

public class goal : MonoBehaviour
{
    public PlayerTeam goalTeam;                // El equipo que recibe el gol si entra aquí
    public GameObject puckPrefab;              // Prefab del puck (asignar en el Inspector)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Puck"))
        {
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

        }
    }

}

