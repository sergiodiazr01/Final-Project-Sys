using UnityEngine;

public class PowerUpBox : MonoBehaviour
{
    public GameObject extraPuckPrefab; 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Puck"))
        {
            PuckColor puck = other.GetComponent<PuckColor>();
            if (puck != null && puck.lastPlayerTouched != null)
            {
                puck.lastPlayerTouched.ActivatePowerUp();
                
                int randomPower = 2;//Random.Range(1, 4);  // 1, 2, o 3
                if(randomPower == 2)
                {
                    // Power-Up 2: Crear un nuevo puck
                    if (extraPuckPrefab != null)
                    {   
                        
                        Vector3 spawnPos = new Vector3(3.867148f, 35.30695f,45.82728f); //coordenadas del centro del campo
                        Instantiate(extraPuckPrefab, spawnPos, Quaternion.identity);
                        Debug.Log("Power-Up: Puck Extra");
                    }
                }
                
                /*    
                switch (randomPower)
                {
                    case 1:
                        // Power-Up 1: 

                    case 2:
                        // Power-Up 2: Crear un nuevo puck
                        
                        if (extraPuckPrefab != null)
                        {
                            Vector3 spawnPos = new Vector3(3.867148f, 35.30695f,45.82728f);
                            Instantiate(extraPuckPrefab, spawnPos, Quaternion.identity);
                            Debug.Log("Power-Up: Puck Extra");
                        }
                        break;

                    case 3:
                        // Power-Up 3: Podría ser otro como escudo o confusión
                        Debug.Log("Power-Up: Escudo (o lo que sea)");
                        break;
                }*/
            }
            }

            // Desactivar la caja tras recogerla
            gameObject.SetActive(false);
        }
}

