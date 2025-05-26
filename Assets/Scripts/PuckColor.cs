using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PuckColor : MonoBehaviour
{
    #region Inspector ▸ Puck
    [Header("Puck Settings")]
    public Renderer puckRenderer;
    public Color defaultColor = Color.white;
    public Color redPlayerColor   = Color.red;
    public Color bluePlayerColor  = Color.blue;
    public PlayerController lastPlayerTouched;
    #endregion

    #region Inspector ▸ Physics
    public float hitforce = 100f;                 // Fuerza del golpe
    public float speedZoneMultiplier = 5f;        // Boost en zona de velocidad
    private const float speedBoostDuration = 3f;  // Duración del boost
    #endregion

    #region Inspector ▸ Audio
    public AudioClip hitByPlayerSound;
    public AudioClip wallBounceSound;
    #endregion

    #region Inspector ▸ Hit FX (Onomatopoeia)
    [Tooltip("Prefabs con TextMeshPro que muestran BAM / PUM / ¡POW! …")]
    public GameObject[] hitOnomatopoeiaPrefabs;
    [Tooltip("Separación para evitar que el texto se incruste en la superficie")]
    public float onomatopoeiaSurfaceOffset = 0.06f;
    public float onomatopoeiaLifetime      = 1.2f;

    [Header("Orientación del texto")]
    [Tooltip("Giro en X que tumba el texto para verse perfectamente desde arriba (90° por defecto)")]
    public float textPitch = 90f;
    [Tooltip("Ángulo Yaw (Y) cuando golpea el Equipo Rojo → apunta a su lado")] 
    public float redYaw  =  90f;
    [Tooltip("Ángulo Yaw (Y) cuando golpea el Equipo Azul → apunta a su lado")] 
    public float blueYaw = -90f;
    #endregion

    private Rigidbody   rb;
    private AudioSource audioSource;

    // -------------------------------------------------- UNITY --------------------------------------------------
    void Start()
    {
        puckRenderer.material.color = defaultColor;
        rb          = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 1. ¿Colisionó con un jugador?
        if (!IsPlayer(collision.gameObject))
        {
            // Pared u otro objeto
            if (collision.gameObject.CompareTag("wall") && audioSource && wallBounceSound)
                audioSource.PlayOneShot(wallBounceSound);
            return;
        }

        // 2. Aplicar fuerza al puck
        lastPlayerTouched = collision.gameObject.GetComponent<PlayerController>();
        Vector3 direction = (rb.position - collision.contacts[0].point).normalized;
        rb.AddForce(direction * hitforce, ForceMode.Impulse);
        if (audioSource && hitByPlayerSound) audioSource.PlayOneShot(hitByPlayerSound);

        // 3. Cambiar color del puck según equipo
        if (lastPlayerTouched != null)
        {
            puckRenderer.material.color = (lastPlayerTouched.GetTeam() == PlayerTeam.Red)
                                          ? redPlayerColor
                                          : bluePlayerColor;
        }

        // 4. FX (onomatopeya)
        SpawnHitFX(collision.contacts[0]);
    }

    // -------------------------------------------------- FX --------------------------------------------------
    void SpawnHitFX(ContactPoint contact)
    {
        if (hitOnomatopoeiaPrefabs == null || hitOnomatopoeiaPrefabs.Length == 0) return;
        int prefabIndex = Random.Range(0, hitOnomatopoeiaPrefabs.Length);

        // Posición exacta del impacto, desplazada hacia afuera
        Vector3 spawnPos = contact.point + contact.normal * onomatopoeiaSurfaceOffset;

        // Yaw depende del último jugador que tocó el puck
        float yaw = 0f;
        if (lastPlayerTouched != null)
            yaw = (lastPlayerTouched.GetTeam() == PlayerTeam.Red) ? redYaw : blueYaw;

        // Rotación final:   (pitch ,  yaw , roll)  -> sólo utilizamos pitch y yaw
        Quaternion rot = Quaternion.Euler(textPitch, yaw, 0f);

        // Instancia del FX
        GameObject fx = Instantiate(hitOnomatopoeiaPrefabs[prefabIndex], spawnPos, rot);

        // Color del texto según equipo
        TextMeshPro tmp = fx.GetComponentInChildren<TextMeshPro>();
        if (tmp != null)
            tmp.color = (lastPlayerTouched != null && lastPlayerTouched.GetTeam() == PlayerTeam.Red)
                         ? redPlayerColor
                         : bluePlayerColor;

        Destroy(fx, onomatopoeiaLifetime);
    }

    // -------------------------------------------------- ZONAS ESPECIALES --------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpeedZone") && !speedBoostCoroutineRunning)
        {
            rb.velocity *= speedZoneMultiplier;
            StartCoroutine(EndSpeedBoostAfterTime());
        }
        else if (other.CompareTag("ReverseZone"))
        {
            Vector3 dir = rb.velocity.normalized;
            rb.velocity        = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(-dir * 15f, ForceMode.Impulse);
        }
    }

    private bool speedBoostCoroutineRunning = false;
    private IEnumerator EndSpeedBoostAfterTime()
    {
        speedBoostCoroutineRunning = true;
        yield return new WaitForSeconds(speedBoostDuration);
        rb.velocity /= speedZoneMultiplier;
        speedBoostCoroutineRunning = false;
    }

    // -------------------------------------------------- HELPERS --------------------------------------------------
    private bool IsPlayer(GameObject go) =>
        go.CompareTag("Player") || go.CompareTag("PlayerRed") || go.CompareTag("PlayerBlue");
}
