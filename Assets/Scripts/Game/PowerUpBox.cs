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

    [Header("Materiales para cada caso")]
    public Material powerUpShield;
    public Material player4Material;

    public Material powerUpIncrease;
    public Material revZoneMaterial;

    public Material powerUpDuplicate;
    public Material player3Material;

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
                    AssignThreeMaterials(powerUpShield, powerUpShield, player4Material);
                    break;
                case 2:  // Duplicar puck
                    AssignThreeMaterials(powerUpDuplicate, powerUpDuplicate, revZoneMaterial);
                    break;
                case 3:  // Crecimiento
                    AssignThreeMaterials(powerUpIncrease, powerUpIncrease, player3Material);
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

            //Renderer rend = GetComponent<Renderer>();

            if (powerType == 1)
            {
                
                if (puckScript != null && puckScript.lastPlayerTouched != null)
                {
                    puckScript.lastPlayerTouched.PlaySoundShield();
                    puckScript.lastPlayerTouched.ActivateGoalShield();
                    Debug.Log("Power-Up: Escudo activado en portería del jugador");
                }
                //rend.material = powerUpShield;
            }
            else if (powerType == 2)
            {

                GameObject newPuck = Instantiate(extraPuckPrefab, other.transform.position, Quaternion.identity);

                if (puckContainer != null)
                {
                    newPuck.transform.SetParent(puckContainer);
                }
                // Obtenemos el script PuckColor del nuevo puck
                PuckColor newPuckScript = newPuck.GetComponent<PuckColor>();

                Rigidbody rb = newPuck.GetComponent<Rigidbody>();
                
                if (newPuckScript != null)
                {
                    Transform redGoal = GameObject.Find("redGoal")?.transform;
                    Transform blueGoal = GameObject.Find("blueGoal")?.transform;

                    if (redGoal != null && blueGoal != null)
                    {
                        newPuckScript.redGoalTarget = redGoal;
                        newPuckScript.blueGoalTarget = blueGoal;
                    }
                    else
                    {
                        Debug.LogWarning("No se encontraron los targets de portería");
                    }
                }

                Debug.Log("Nuevo puck creado");

                newPuckScript.ActivateAutoDestruct(extraPuckLifetime);
            }
            else if (powerType == 3)
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

    private void AssignThreeMaterials(Material m0, Material m1, Material m2)
    {
        int slots = 3;
        Material[] arr = new Material[slots];
        arr[0] = m0;
        arr[1] = m1;
        arr[2] = m2;
        boxRenderer.materials = arr;
    }



}
