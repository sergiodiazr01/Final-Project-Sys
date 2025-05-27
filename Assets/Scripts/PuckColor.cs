using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PuckColor : MonoBehaviour
{
    public Renderer puckRenderer;

    public Color defaultColor = Color.white;
    public Color redPlayerColor = Color.red;
    public Color bluePlayerColor = Color.blue;
    public PlayerController lastPlayerTouched;

    public float speed = 10f;
    public float maxVelocity = 20f;

    public float speedZoneMultiplier = 5f; // Multiplicador de velocidad en la zona de velocidad

    private Rigidbody rb;
    public float hitforce = 100f; // Fuerza de golpeo del puck

    private bool isSpeedBoosted = false;
    private float speedBoostDuration = 3f;
    public float linearFriction = 0.99f; // 1 = sin fricción, 0 = freno total

    public AudioClip hitByPlayerSound;
    public AudioClip wallBounceSound;
    private AudioSource audioSource;

    public AudioClip speedSound;
    public AudioClip teledirigidaSound;
    public AudioClip inverseSound;
    public Transform redGoalTarget;
    public Transform blueGoalTarget;
    public float forceToGoal = 10f;

    private bool isDirectionInverted = false;
    public float invertDuration = 5f;

    private bool wasInRepulsorZone = false;
    private Vector3 lastDirectionBeforeZone;


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



    void Start()
    {
        puckRenderer.material.color = defaultColor;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        if (redGoalTarget == null)
        {
            GameObject red = GameObject.Find("redGoal");
            if (red != null) redGoalTarget = red.transform;
        }

        if (blueGoalTarget == null)
        {
            GameObject blue = GameObject.Find("blueGoal");
            if (blue != null) blueGoalTarget = blue.transform;
        }
    }

    void FixedUpdate()
    {
        // Aplicar fricción artificial
        rb.velocity *= linearFriction;

        // Limita la velocidad máxima
        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            lastPlayerTouched = collision.gameObject.GetComponent<PlayerController>();
            Debug.Log(lastPlayerTouched);

            Vector3 direction = (rb.position - collision.contacts[0].point).normalized;
            //float impactSpeed = collision.relativeVelocity.magnitude;
            //float scaledForce = impactSpeed * hitforce;
            //rb.AddForce(direction * scaledForce, ForceMode.Impulse);
            if (isDirectionInverted)
            {
                direction.x *= -1f;
                Debug.Log("Dirección horizontal invertida");
            }
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
            // Spawn hit FX
            SpawnHitFX(collision.contacts[0]);
        }
        else if (collision.gameObject.CompareTag("wall"))
        {
            if (audioSource != null && wallBounceSound != null)
                audioSource.PlayOneShot(wallBounceSound);
        }


    }

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpeedZone") && !isSpeedBoosted)
        {
            if (audioSource != null && speedSound != null)
            {
                audioSource.PlayOneShot(speedSound);  
            }
                
            Debug.Log("Puck ha entrado en zona de velocidad");

            rb.velocity *= speedZoneMultiplier;
            isSpeedBoosted = true;
        }
        else if (other.CompareTag("RepulsorZone"))
        {
            if (audioSource != null && inverseSound != null)
            {
                audioSource.PlayOneShot(inverseSound);  
            }
            Debug.Log("Puck ha entrado en RepulsorZone");

            if (rb.velocity.magnitude > 0.1f)
            {
                Vector3 baseDir = -rb.velocity.normalized;
                Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
                Vector3 finalDir = (baseDir + randomOffset).normalized;

                float reboundForce = rb.velocity.magnitude * 2.2f; 

                rb.velocity = finalDir * reboundForce;
                rb.angularVelocity = -rb.angularVelocity;

                
            }
        }

        else if (other.CompareTag("TeledirigidaZone") && lastPlayerTouched != null)
        {
            if (audioSource != null && teledirigidaSound != null)
            {
                audioSource.PlayOneShot(teledirigidaSound);  
            }
            Debug.Log("Puck ha entrado en zona teledirigida");

            PlayerTeam team = lastPlayerTouched.GetTeam();
            Transform target;

            if (team == PlayerTeam.Red)
            {
                target = blueGoalTarget;
            }
            else
            {
                target = redGoalTarget;
            }


            Vector3 direction = (target.position - rb.position).normalized;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(direction * forceToGoal, ForceMode.Impulse);
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
private IEnumerator InvertDirectionTemporarily()
{
    isDirectionInverted = true;
    yield return new WaitForSeconds(invertDuration);
    isDirectionInverted = false;
    Debug.Log("Dirección del puck restaurada a normal");
}

    public void ActivateAutoDestruct(float time)
    {
        StartCoroutine(AutoDestruct(time));
    }

    private IEnumerator AutoDestruct(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("Puck extra autodestruido");
        Destroy(gameObject);
    }

}
