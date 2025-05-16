using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckColor : MonoBehaviour
{
    public Renderer puckRenderer;

    public Color defaultColor = Color.white;
    public Color redPlayerColor = Color.red;
    public Color bluePlayerColor = Color.blue;
    public PlayerController lastPlayerTouched;

    public float speed = 10f;
    public float speedZoneMultiplier = 5f; // Multiplicador de velocidad en la zona de velocidad

    private Rigidbody rb;
    public float hitforce = 100f; // Fuerza de golpeo del puck

    private bool isSpeedBoosted = false;
    private float speedBoostDuration = 3f;  
    
    public AudioClip hitByPlayerSound;
    public AudioClip wallBounceSound;
    private AudioSource audioSource;

    void Start()
    {
        puckRenderer.material.color = defaultColor;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            lastPlayerTouched = collision.gameObject.GetComponent<PlayerController>();

            Vector3 direction = (rb.position - collision.contacts[0].point).normalized;
            //float impactSpeed = collision.relativeVelocity.magnitude;
            //float scaledForce = impactSpeed * hitforce;
            //rb.AddForce(direction * scaledForce, ForceMode.Impulse);
            rb.AddForce(direction * hitforce, ForceMode.Impulse);
            audioSource.PlayOneShot(hitByPlayerSound);

            /*if (collision.gameObject.CompareTag("PlayerRed"))
            {
                Debug.Log("a");
                puckRenderer.material.color = redPlayerColor;
            }
            else if (collision.gameObject.CompareTag("PlayerBlue"))
            {
                puckRenderer.material.color = bluePlayerColor;
            }*/
            if (lastPlayerTouched != null)
            {
                if (lastPlayerTouched.GetTeam() == PlayerTeam.Red)
                    puckRenderer.material.color = redPlayerColor;
                else if (lastPlayerTouched.GetTeam() == PlayerTeam.Blue)
                    puckRenderer.material.color = bluePlayerColor;
            }

        }
        else if (collision.gameObject.CompareTag("wall"))
        {
            if (audioSource != null && wallBounceSound != null)
                audioSource.PlayOneShot(wallBounceSound);
        }

}
private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("SpeedZone") && !isSpeedBoosted)
    {
        Debug.Log("Puck ha entrado en zona de velocidad");

        rb.velocity *= speedZoneMultiplier;
        isSpeedBoosted = true;
    }
}

private void OnTriggerExit(Collider other)
{
    if (other.CompareTag("SpeedZone"))
    {
        Debug.Log("Puck ha salido de la zona de velocidad");

        // Al salir, empieza el conteo para volver a velocidad normal
        StartCoroutine(EndSpeedBoostAfterTime());
    }
}

private IEnumerator EndSpeedBoostAfterTime()
{
    yield return new WaitForSeconds(speedBoostDuration);

    // Reducir la velocidad (de forma proporcional)
    rb.velocity /= speedZoneMultiplier;
    isSpeedBoosted = false;

    Debug.Log("Puck vuelve a velocidad normal");
}

}
