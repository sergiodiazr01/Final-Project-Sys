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
    private Vector3 originalScale;
    private CapsuleCollider capsule;
    private float originalHeight;
    private float originalRadius;
    public GameObject goalShield;
    public bool isSizeBoosted = false;
    
    private void Start()
    {
        originalScale = transform.localScale;
        capsule = GetComponent<CapsuleCollider>();
        if (capsule != null)
        {
            originalHeight = capsule.height;
            originalRadius = capsule.radius;
        }
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

    public void ActivateSizeBoost(float multiplier = 1.5f, float duration = 5f)
    {
        if (isSizeBoosted) return;
        StartCoroutine(SizeBoostCoroutine(multiplier, duration));
    }

    private IEnumerator SizeBoostCoroutine(float multiplier, float duration)
    {
        isSizeBoosted = true;
        Vector3 originalScale = transform.localScale;
        transform.localScale = originalScale * multiplier;
        // Aumentar también el collider
        if (capsule != null)
        {
            capsule.height = originalHeight * multiplier;
            capsule.radius = originalRadius * multiplier;
        }

        Debug.Log("¡Player aumentado!");

        yield return new WaitForSeconds(duration);

        transform.localScale = originalScale;
        Debug.Log("Tamaño del player restaurado.");
        // Restaurar collider
        if (capsule != null)
        {
            capsule.height = originalHeight;
            capsule.radius = originalRadius;
        }
        isSizeBoosted = false;
    }
    
    public void ActivateGoalShield(float duration = 5f)
    {
        if (goalShield != null)
            StartCoroutine(ShieldCoroutine(duration));
    }

    private IEnumerator ShieldCoroutine(float duration)
    {
        Collider shieldCollider = goalShield.GetComponent<Collider>();
        
        if (shieldCollider != null)
            shieldCollider.enabled = true;  // ACTIVAR el collider

        goalShield.SetActive(true); // (opcional si también quieres mostrar algo visual)
        Debug.Log("Escudo activado en la portería de " + team);

        yield return new WaitForSeconds(duration);

        if (shieldCollider != null)
            shieldCollider.enabled = false;  // DESACTIVAR el collider

        goalShield.SetActive(false); // (opcional para ocultar el objeto)
        Debug.Log("Escudo desactivado");
    }



}
