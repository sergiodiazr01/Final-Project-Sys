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

    private bool isPoweredUp = false;     //controla si tiene powerup activo
    private Vector3 originalScale;
    private CapsuleCollider capsule;
    private float originalHeight;
    private float originalRadius;
    public GameObject goalShield;
    public bool isSizeBoosted = false;

    //variables crecimiento porteria
    public Transform ownGoal;                
    public float proximityDistance = 10f;    //distancia para empezar a contar
    public float timeToGrow = 5f;            //tiempo que debe estar cerca
    public float enlargedZScale = 22f;        //valor Z al que crece la porteria
    public float resizeDuration = 3f;        //tiempo que tarda en crecer

    private float timeNearGoal = 0f;
    private bool isGrowing = false;
    private Coroutine resizeCoroutine = null;
    private Vector3 originalGoalScale;

    private AudioSource audioSource;
    public AudioClip growSound; //sonido de crecer
    public AudioClip decreaseSound; //sonido de decrecer

    public AudioClip powerUpSound; //sonido de powerup
    public AudioClip sizeBoostSound; //sonido de aumento de tamaño player
    public AudioClip shieldSound; //sonido del escudo

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
                audioSource.PlayOneShot(growSound); 
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
                audioSource.PlayOneShot(decreaseSound); 
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

        yield return new WaitForSeconds(5f);  
        isPoweredUp = false;
        Debug.Log($"[Player {team}] Power-Up finalizado.");
    }

    // Getter para saber que equipo es este jugador
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
        //aumentar tambien el collider
        if (capsule != null)
        {
            capsule.height = originalHeight * multiplier;
            capsule.radius = originalRadius * multiplier;
        }

        Debug.Log("¡Player aumentado!");

        yield return new WaitForSeconds(duration);

        transform.localScale = originalScale;
        Debug.Log("Tamaño del player restaurado.");
        //restaurar collider
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
            shieldCollider.enabled = true;  //ACTIVAR el collider

        goalShield.SetActive(true); 
        Debug.Log("Escudo activado en la portería de " + team);

        yield return new WaitForSeconds(duration);

        if (shieldCollider != null)
            shieldCollider.enabled = false;  //DESACTIVAR el collider

        goalShield.SetActive(false); 
        Debug.Log("Escudo desactivado");
    }

    public void PlaySoundPowerUp()
    {
        if (audioSource != null && powerUpSound != null)
        {
            audioSource.PlayOneShot(powerUpSound);
        }
    }

    public void PlaySoundGrow()
    {
        if (audioSource != null && growSound != null)
        {
            audioSource.PlayOneShot(growSound);
        }
    }

    public void PlaySoundShield()
    {
        if (audioSource != null && shieldSound != null)
        {
            audioSource.PlayOneShot(shieldSound);
        }
    }

}
