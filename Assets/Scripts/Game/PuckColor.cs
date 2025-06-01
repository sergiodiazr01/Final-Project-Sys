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

    public float hitMultiplier = 1.2f;   
    public float wallElasticity = 0.9f; 

    //cooldown entre golpes para evitar múltiples OnCollisionEnter en el mismo contacto
    private float _lastHitTime = -1f;
    [Tooltip("Segundos mínimos entre dos golpes sucesivos")]
    public float hitCooldown = 0.5f;

    //velocidad minima para considerar impacto
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

    [Header("FX – Partículas zonas")]
    public ParticleSystem redImpactParticlePrefab;
    public ParticleSystem blueImpactParticlePrefab;
    public ParticleSystem speedZoneParticlePrefab;
    public ParticleSystem repulsorZoneParticlePrefab;
    public ParticleSystem teledirigidaZoneParticlePrefab;
    public float zoneParticleLifetime = 1.0f;
    public float particleLifetime = 1.5f;

    [Header("Progresión de velocidad")]
    [Tooltip("Aumento de multiplicador por segundo de juego")]
    public float speedIncreaseRate = 0.08f; //+8% por segundo al puck
    private float gameTime = 0f;
    private float gameSpeedMultiplier = 1f;

    private int lastOnomatopoeiaIndex = -1;

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
        
        gameTime += Time.fixedDeltaTime;
        gameSpeedMultiplier = 1f + gameTime * speedIncreaseRate;

        //friccion
        rb.velocity *= linearFriction;

        //limitar la velocidad maxima
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
            //si chocamos de nuevo antes de hitCooldown, salimos
            if (Time.time - _lastHitTime < hitCooldown) return;
            _lastHitTime = Time.time;

            //velocidad del paddle (el player)
            var tracker = collision.gameObject.GetComponent<PlayerVelocityTracker>();
            Vector3 paddleVel = tracker?.SmoothedVelocity ?? Vector3.zero;
            if (paddleVel.magnitude < minHitSpeed) return;  // filtro de golpes suaves

            //direccion del golpe
            Vector3 hitDir = (rb.position - collision.transform.position).normalized;

            //magnitud del nuevo vector
            float rawSpeed = paddleVel.magnitude * hitMultiplier;
            rawSpeed *= gameSpeedMultiplier;
            //aplicar velocidad minima
            float newSpeed = Mathf.Max(rawSpeed, minPuckHitSpeed);

            rb.velocity = hitDir * newSpeed;

            //pequeño empujon extra para “salir” del collider
            rb.position += hitDir * 0.02f;

            // FX
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
            //limitar rebote a cada wallHitCooldown segundos
            if (Time.time - _lastWallHitTime < wallHitCooldown)
                return;
            _lastWallHitTime = Time.time;

            ContactPoint contact = collision.contacts[0];
            Vector3 normal = contact.normal;
            Vector3 v = rb.velocity;

           
            Vector3 vNormal = Vector3.Dot(v, normal) * normal;
            Vector3 vTangent = v - vNormal;

           
            Vector3 reflectedNormal = -vNormal * wallElasticity;

            
            Vector3 newVelocity = vTangent + reflectedNormal;

            //velocidad minima
            float minBounceSpeed = 10f;
            if (newVelocity.magnitude < minBounceSpeed)
                newVelocity = newVelocity.normalized * minBounceSpeed;

            rb.velocity = newVelocity;

            //separamos un poco el puck para evitar colisiones repetidas
            float penetrationOffset = 0f;
            rb.position += normal * penetrationOffset;

            audioSource.PlayOneShot(wallBounceSound);
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            //cooldown entre rebotes con obstaculos
            if (Time.time - _lastWallHitTime < wallHitCooldown)
                return;
            _lastWallHitTime = Time.time;

            ContactPoint contact = collision.contacts[0];
            Vector3 normal = contact.normal;
            Vector3 v = rb.velocity;

            
            Vector3 vNormal = Vector3.Dot(v, normal) * normal;
            Vector3 vTangent = v - vNormal;

           
            float obstacleElasticity = 0.8f; 
            Vector3 reflectedNormal = -vNormal * obstacleElasticity;

           //esta sera la velocidad maxima al final
            Vector3 newVelocity = vTangent + reflectedNormal;

            //velocidad minima rebote
            float minObstacleBounce = 8f;
            if (newVelocity.magnitude < minObstacleBounce)
                newVelocity = newVelocity.normalized * minObstacleBounce;

            rb.velocity = newVelocity;

            //separar un poco al puck del obstáculo
            float penetrationOffset = 0.3f;  
            rb.position += normal * penetrationOffset;

            
            if (audioSource != null && wallBounceSound != null)
                audioSource.PlayOneShot(wallBounceSound);

            return;
        }


    }

    void SpawnHitFX(ContactPoint contact)
    {
        if (hitOnomatopoeiaPrefabs == null || hitOnomatopoeiaPrefabs.Length == 0) return;
        int prefabIndex;
        do
        {
            prefabIndex = Random.Range(0, hitOnomatopoeiaPrefabs.Length);
        } while (hitOnomatopoeiaPrefabs.Length > 1 && prefabIndex == lastOnomatopoeiaIndex);
        lastOnomatopoeiaIndex = prefabIndex;

        //posicion exacta del impacto, vector hacia afuera
        Vector3 spawnPos = new Vector3(0, 10, 0) + contact.point + contact.normal * onomatopoeiaSurfaceOffset;

        //yaw depende del último jugador que tocó el puck
        float yaw = 0f;
        if (lastPlayerTouched != null)
            yaw = (lastPlayerTouched.GetTeam() == PlayerTeam.Red) ? redYaw : blueYaw;

        // Rotacion final:   (pitch ,  yaw , roll)  (solo utilizamos pitch y yaw)
        Quaternion rot = Quaternion.Euler(textPitch, yaw, 0f);

        //FX
        GameObject fx = Instantiate(hitOnomatopoeiaPrefabs[prefabIndex], spawnPos, rot);

        //color del texto segun equipo
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
            SpawnZoneParticles(speedZoneParticlePrefab);
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
            SpawnZoneParticles(repulsorZoneParticlePrefab);
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
            SpawnZoneParticles(teledirigidaZoneParticlePrefab);

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SpeedZone"))
        {
            Debug.Log("Puck ha salido de la zona de velocidad");

            //volver a la velocidad normal
            StartCoroutine(EndSpeedBoostAfterTime());
        }



    }

    private IEnumerator EndSpeedBoostAfterTime()
    {
        yield return new WaitForSeconds(speedBoostDuration);

        //reducir la velocidad de forma proporcional
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
    
    void SpawnZoneParticles(ParticleSystem prefab)
    {
        if (!prefab) return;
        Vector3 spawnPos = transform.position + Vector3.up * 0.05f; // un poco encima del puck
        ParticleSystem ps = Instantiate(prefab, spawnPos, Quaternion.identity);
        Destroy(ps.gameObject, zoneParticleLifetime);
    }

}
