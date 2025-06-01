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

    public Material shieldMaterial;
    public Material growMaterial;
    public Material duplicateMaterial;
    private int powerType;
    private Renderer boxRenderer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        boxRenderer = GetComponentInChildren<Renderer>();

        powerType = Random.Range(1, 4);

        // Cambiamos el material
        if (boxRenderer != null)
        {
            switch (powerType)
            {
                case 1:  // Escudo
                    boxRenderer.material = shieldMaterial;
                    break;
                case 2:  // Duplicar puck
                    boxRenderer.material = duplicateMaterial;
                    break;
                case 3:  // Crecimiento
                    boxRenderer.material = growMaterial;
                    break;
            }
        }
    }

    private void Start()
    {
        //if (puckContainer == null)
        //{
        //    GameObject container = GameObject.Find("puckContainer");
        //    if (container != null)
        //    {
        //        puckContainer = container.transform;
        //    }
        //}
        //audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {   
        
        // Comprobar si es un puck
        if (other.CompareTag("Puck"))
        {
            // Reproducir sonido de power-up
            PuckColor puckScript = other.GetComponentInParent<PuckColor>();
            puckScript.lastPlayerTouched.PlaySoundPowerUp();

            // Activar power-up al jugador
            //puck.lastPlayerTouched.ActivatePowerUp();

            int randomPower = Random.Range(1, 4);
            // Forzar solo powerup boxes
            // randomPower = 3;
            Renderer rend = GetComponent<Renderer>();

            if (randomPower == 1)
            {
                
                if (puckScript != null && puckScript.lastPlayerTouched != null)
                {
                    puckScript.lastPlayerTouched.PlaySoundShield();
                    puckScript.lastPlayerTouched.ActivateGoalShield();
                    Debug.Log("Power-Up: Escudo activado en portería del jugador");
                }
                rend.material = shieldMaterial;
            }
            else if (randomPower == 2)
            {

                GameObject newPuck = Instantiate(extraPuckPrefab, other.transform.position, Quaternion.identity);

                if (puckContainer != null)
                {
                    newPuck.transform.SetParent(puckContainer);
                }

                Rigidbody rb = newPuck.GetComponent<Rigidbody>();
                
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
                
                Debug.Log("lastPlayerTouched: " + puckScript.lastPlayerTouched);
                if (puckScript != null && puckScript.lastPlayerTouched != null)
                {
                    puckScript.lastPlayerTouched.PlaySoundGrow();
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
