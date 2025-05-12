using UnityEngine;
using System.Collections;

public enum PlayerTeam
{
    Red,
    Blue
}

public class PlayerController : MonoBehaviour
{
    public PlayerTeam team;
    
    private bool isPoweredUp = false;     // Controla si tiene power-up activo

    private void Start()
    {
        // Asignar automáticamente el equipo según el tag (si prefieres no setearlo manualmente)
        if (gameObject.CompareTag("PlayerRed"))
            team = PlayerTeam.Red;
        else if (gameObject.CompareTag("PlayerBlue"))
            team = PlayerTeam.Blue;
    }

    public void ActivatePowerUp()
    {
        if (!isPoweredUp)
        {
            StartCoroutine(PowerUpCoroutine());
        }
    }

     private IEnumerator PowerUpCoroutine()
    {
        isPoweredUp = true;

        Debug.Log($"[Player {team}] Power-Up activado!");

        // aqui se pueden añadir efectos visuales o de sonido

        yield return new WaitForSeconds(5f);  // Duración del power-up

        isPoweredUp = false;
        Debug.Log($"[Player {team}] Power-Up finalizado.");
    }

    // Getter para saber qué equipo es este jugador
    public PlayerTeam GetTeam()
    {
        return team;
    }
}
