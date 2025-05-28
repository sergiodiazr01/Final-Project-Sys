using UnityEngine;
using System.Collections;
public class PowerUpBox : MonoBehaviour
{
    
    public GameObject extraPuckPrefab;
    public float extraPuckLifetime = 5f; // Tiempo que el puck extra estará activo
    public Transform puckContainer; // Donde guardamos los pucks

    private AudioSource audioSource; // Componente de AudioSource para reproducir sonidos
    public AudioClip powerUpSound; // Sonido del power-up
    public AudioClip GrowSound; // Sonido del puck extra
    public AudioClip ShieldSound; // Sonido del aumento de tamaño
    private void Start()
    {
        if (puckContainer == null)
        {
            GameObject container = GameObject.Find("puckContainer");
            if (container != null)
            {
                puckContainer = container.transform;
            }
        }
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Comprobar si es un puck
        if (other.CompareTag("Puck"))
        {
            // Reproducir sonido de power-up
            if (powerUpSound != null)
            {
                audioSource.PlayOneShot(powerUpSound);
            }
            

            // Activar power-up al jugador
            //puck.lastPlayerTouched.ActivatePowerUp();

            int randomPower = Random.Range(1, 4);
            // Forzar solo powerup boxes
            // randomPower = 3;

            if (randomPower == 1)
                {
                    PuckColor puckScript = other.GetComponentInParent<PuckColor>();
                    if (puckScript != null && puckScript.lastPlayerTouched != null)
                    {   
                        audioSource.PlayOneShot(ShieldSound);
                        puckScript.lastPlayerTouched.ActivateGoalShield();
                        Debug.Log("Power-Up: Escudo activado en portería del jugador");
                    }
                }else if(randomPower == 2)
                {
                    
                    GameObject newPuck = Instantiate(extraPuckPrefab, other.transform.position, Quaternion.identity);

                    if (puckContainer != null)
                    {
                        newPuck.transform.SetParent(puckContainer);
                    }

                    Rigidbody rb = newPuck.GetComponent<Rigidbody>();
                    PuckColor puckScript = newPuck.GetComponent<PuckColor>();
                    if (puckScript != null)
                    {
                        Transform redGoal = GameObject.Find("redGoal")?.transform;
                        Transform blueGoal = GameObject.Find("blueGoal")?.transform;

                        if (redGoal != null && blueGoal != null)
                        {
                            puckScript.redGoalTarget = redGoal;
                            puckScript.blueGoalTarget = blueGoal;
                        }
                        else
                        {
                            Debug.LogWarning("No se encontraron los targets de portería");
                        }
                    }
                    
                    Debug.Log("Nuevo puck creado");

                    puckScript.ActivateAutoDestruct(extraPuckLifetime);
            }
                else if (randomPower == 3)
                {
                    PuckColor puckScript = other.GetComponentInParent<PuckColor>();
                    Debug.Log("lastPlayerTouched: " + puckScript.lastPlayerTouched);
                    if (puckScript != null && puckScript.lastPlayerTouched != null)
                {
                    audioSource.PlayOneShot(GrowSound);
                    puckScript.lastPlayerTouched.ActivateSizeBoost();
                    Debug.Log("Power-Up: Jugador aumentado temporalmente");
                }
                else
                {
                    Debug.LogWarning("No se pudo aplicar el power-up de tamaño: jugador no detectado");
                }
                }

              
            

            //destruir la caja tras tocarla
            Destroy(gameObject);

        }
    }

  

}
