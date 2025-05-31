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

    public float hitMultiplier = 1.2f;   // tweak until it feels right
    public float wallElasticity = 0.9f;  // 1 = perfect, <1 = a bit of damping

    // Cooldown entre golpes para evitar múltiples OnCollisionEnter en el mismo contacto
    private float _lastHitTime = -1f;
    [Tooltip("Segundos mínimos entre dos golpes sucesivos")]
    public float hitCooldown = 0.5f;

    // Umbral de velocidad mínima para considerar un impacto real
    [Tooltip("Ignora golpes si la velocidad del paddle es menor")]
    public float minHitSpeed = 0.5f;
    public float minPuckHitSpeed = 2f;

    [Header("Rebote pared")]
    [Tooltip("Segundos mínimos entre dos rebotes contra la pared")]
    public float wallHitCooldown = 0.5f;
    private float _lastWallHitTime = -Mathf.Infinity;



    [Tooltip("Prefabs con TextMeshPro que muestran BAM / PUM / ¡POW! …")]
    public GameObject[] hitOnomatopoeiaPrefabs;
    [Tooltip("Separación para evitar que el texto se incruste en la superficie")]
    public float onomatopoeiaSurfaceOffset = 0.06f;
    public float onomatopoeiaLifetime = 1.2f;

    [Header("Orientación del texto")]
    [Tooltip("Giro en X que tumba el texto para verse perfectamente desde arriba (90° por defecto)")]
    public float textPitch = 90f;
    [Tooltip("Ángulo Yaw (Y) cuando golpea el Equipo Rojo → apunta a su lado")]
    public float redYaw = 90f;
    [Tooltip("Ángulo Yaw (Y) cuando golpea el Equipo Azul → apunta a su lado")]
    public float blueYaw = -90f;

    public ParticleSystem redImpactParticlePrefab;
    public ParticleSystem blueImpactParticlePrefab;

    public float particleLifetime = 1.5f;

    [Header("Progresión de velocidad")]
    [Tooltip("Aumento de multiplicador por segundo de juego")]
    public float speedIncreaseRate = 0.08f; // +8% por segundo
    private float gameTime = 0f;
    private float gameSpeedMultiplier = 1f;


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
        // 0) Actualiza temporizador y multiplicador
        gameTime += Time.fixedDeltaTime;
        gameSpeedMultiplier = 1f + gameTime * speedIncreaseRate;

        // Aplicar fricción artificial
        rb.velocity *= linearFriction;

        // Limita la velocidad máxima
        float scaledMaxVel = maxVelocity * gameSpeedMultiplier;
        if (scaledMaxVel > 120f)
        {
            scaledMaxVel = 120f; // Limita la velocidad máxima a 180 unidades
        }
        
        if (rb.velocity.magnitude > scaledMaxVel)
        {
            rb.velocity = rb.velocity.normalized * scaledMaxVel;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 1) Cooldown: si chocamos de nuevo antes de hitCooldown, salimos
            if (Time.time - _lastHitTime < hitCooldown) return;
            _lastHitTime = Time.time;

            // 2) Velocidad del paddle
            var tracker = collision.gameObject.GetComponent<PlayerVelocityTracker>();
            Vector3 paddleVel = tracker?.SmoothedVelocity ?? Vector3.zero;
            if (paddleVel.magnitude < minHitSpeed) return;  // filtro de golpes suaves

            // 3) Dirección del golpe
            Vector3 hitDir = (rb.position - collision.transform.position).normalized;

            // 4) Magnitud del nuevo vector
            float rawSpeed = paddleVel.magnitude * hitMultiplier;
            rawSpeed *= gameSpeedMultiplier;
            // Aplica velocidad mínima
            float newSpeed = Mathf.Max(rawSpeed, minPuckHitSpeed);

            rb.velocity = hitDir * newSpeed;

            // 5) Pequeño empujón extra para “salir” del collider
            rb.position += hitDir * 0.02f;

            // — el resto de tu FX, sonido y color sale igual —
            lastPlayerTouched = collision.gameObject.GetComponent<PlayerController>();
            puckRenderer.material.color = lastPlayerTouched.GetTeam() == PlayerTeam.Red
                                          ? redPlayerColor : bluePlayerColor;
            audioSource.PlayOneShot(hitByPlayerSound);
            SpawnHitFX(collision.contacts[0]);
            SpawnImpactParticles(collision.contacts[0]);
            return;
        }
        else if (collision.gameObject.CompareTag("wall"))
        {
            // 0) Cooldown: solo un rebote cada wallHitCooldown segundos
            if (Time.time - _lastWallHitTime < wallHitCooldown)
                return;
            _lastWallHitTime = Time.time;

            ContactPoint contact = collision.contacts[0];
            Vector3 normal = contact.normal;
            Vector3 v = rb.velocity;

            // 1) Descompón la velocidad en normal y tangente
            Vector3 vNormal = Vector3.Dot(v, normal) * normal;
            Vector3 vTangent = v - vNormal;

            // 2) Refleja solo la componente normal e invierte su signo
            Vector3 reflectedNormal = -vNormal * wallElasticity;

            // 3) Reconstruye la velocidad: conservas la tangencial (sin fricción)
            Vector3 newVelocity = vTangent + reflectedNormal;

            // 4) (Opcional) fuerza una velocidad mínima si quieres:
            float minBounceSpeed = 10f;
            if (newVelocity.magnitude < minBounceSpeed)
                newVelocity = newVelocity.normalized * minBounceSpeed;

            rb.velocity = newVelocity;

            // 5) Separa un poco el puck para evitar colisiones repetidas
            float penetrationOffset = 0f;
            rb.position += normal * penetrationOffset;

            audioSource.PlayOneShot(wallBounceSound);
        }
    else if (collision.gameObject.CompareTag("Obstacle"))
        {
            // 3.1) (Opcional) Cooldown entre rebotes con obstáculos
            if (Time.time - _lastWallHitTime < wallHitCooldown)
                return;
            _lastWallHitTime = Time.time;

            ContactPoint contact = collision.contacts[0];
            Vector3 normal = contact.normal;
            Vector3 v = rb.velocity;

            // 3.2) Descomponer igual que con la pared
            Vector3 vNormal  = Vector3.Dot(v, normal) * normal;
            Vector3 vTangent = v - vNormal;

            // 3.3) Reflejar la normal con elasticidad de obstáculo (quizá distinta a la pared)
            float obstacleElasticity = 0.8f;  // por ejemplo, 0.8 para un rebote menos elástico
            Vector3 reflectedNormal = -vNormal * obstacleElasticity;

            // 3.4) Reconstruir velocidad
            Vector3 newVelocity = vTangent + reflectedNormal;

            // 3.5) (Opcional) Velocidad mínima de rebote
            float minObstacleBounce = 8f;
            if (newVelocity.magnitude < minObstacleBounce)
                newVelocity = newVelocity.normalized * minObstacleBounce;

            rb.velocity = newVelocity;

            // 3.6) Separar un poco al puck fuera del obstáculo
            float penetrationOffset = 0.3f;  // un poco menos que con la pared
            rb.position += normal * penetrationOffset;

            // 3.7) Sonido de rebote contra obstáculo (puedes usar el mismo que la pared o otro distinto)
            if (audioSource != null && wallBounceSound != null)
                audioSource.PlayOneShot(wallBounceSound);

            return;
        }


    }

    void SpawnHitFX(ContactPoint contact)
    {
        if (hitOnomatopoeiaPrefabs == null || hitOnomatopoeiaPrefabs.Length == 0) return;
        int prefabIndex = Random.Range(0, hitOnomatopoeiaPrefabs.Length);

        // Posición exacta del impacto, desplazada hacia afuera
        Vector3 spawnPos = new Vector3(0, 10, 0) + contact.point + contact.normal * onomatopoeiaSurfaceOffset;

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
    
   void SpawnImpactParticles(ContactPoint contact)
    {
        ParticleSystem prefab = (lastPlayerTouched.GetTeam() == PlayerTeam.Red)
                                 ? redImpactParticlePrefab
                                 : blueImpactParticlePrefab;
        if (!prefab) return;

        Vector3 spawnPos = contact.point + contact.normal * 0.02f;
        Quaternion rot = Quaternion.LookRotation(contact.normal);
        ParticleSystem ps = Instantiate(prefab, spawnPos, rot);
        Destroy(ps.gameObject, particleLifetime);
    }

}
