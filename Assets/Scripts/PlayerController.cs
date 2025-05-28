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

    //variables crecimiento porteria
    public Transform ownGoal;                // Asignar portería desde el Inspector
    public float proximityDistance = 10f;    // Distancia para empezar a contar
    public float timeToGrow = 5f;            // Tiempo que debe estar cerca
    public float enlargedZScale = 22f;        // Valor Z al que crecerá la portería
    public float resizeDuration = 3f;        // Tiempo de la animación

    private float timeNearGoal = 0f;
    private bool isGrowing = false;
    private Coroutine resizeCoroutine = null;
    private Vector3 originalGoalScale;

    private AudioSource audioSource; // Componente de AudioSource para reproducir sonidos
    public AudioClip growSound; // Sonido de crecer
    public AudioClip decreaseSound; // Sonido de decrecer


    private void Start()
    {
        originalScale = transform.localScale;
        capsule = GetComponent<CapsuleCollider>();
        if (capsule != null)
        {
            originalHeight = capsule.height;
            originalRadius = capsule.radius;
        }
        if (ownGoal != null)
        {
            originalGoalScale = ownGoal.localScale;
        }
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (ownGoal == null) return;

        float distance = Vector3.Distance(transform.position, ownGoal.position);

        if (distance < proximityDistance)
        {
            timeNearGoal += Time.deltaTime;
            Debug.Log($"[PORTERÍA] Cerca durante {timeNearGoal:F2}s");

            if (!isGrowing && timeNearGoal >= timeToGrow)
            {
                if (resizeCoroutine != null) StopCoroutine(resizeCoroutine);
                resizeCoroutine = StartCoroutine(ResizeGoalZ(originalGoalScale.z, enlargedZScale));
                isGrowing = true;
                audioSource.PlayOneShot(growSound); // Reproducir sonido de crecimiento
                Debug.Log("[PORTERÍA] Creciendo...");
            }
        }
        else
        {
            if (timeNearGoal > 0f)
            {
                Debug.Log("[PORTERÍA] Jugador se ha alejado, contador reiniciado");
            }

            timeNearGoal = 0f;

            if (isGrowing)
            {
                if (resizeCoroutine != null) StopCoroutine(resizeCoroutine);
                resizeCoroutine = StartCoroutine(ResizeGoalZ(ownGoal.localScale.z, originalGoalScale.z));
                isGrowing = false;
                audioSource.PlayOneShot(decreaseSound); // Reproducir sonido de decrecimiento
                Debug.Log("[PORTERÍA] Volviendo al tamaño original...");
            }
        }
    }

    private IEnumerator ResizeGoalZ(float fromZ, float toZ)
    {
        float elapsed = 0f;
        Vector3 startScale = ownGoal.localScale;
        Vector3 targetScale = new Vector3(startScale.x, startScale.y, toZ);

        while (elapsed < resizeDuration)
        {
            float t = elapsed / resizeDuration;
            float newZ = Mathf.Lerp(fromZ, toZ, t);
            ownGoal.localScale = new Vector3(startScale.x, startScale.y, newZ);
            elapsed += Time.deltaTime;
            yield return null;
        }

        ownGoal.localScale = targetScale;
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
