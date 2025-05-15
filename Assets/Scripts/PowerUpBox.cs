using UnityEngine;
using System.Collections;
public class PowerUpBox : MonoBehaviour
{
    
    public GameObject extraPuckPrefab;
   

    private void OnTriggerEnter(Collider other)
    {
        // Comprobar si es un puck
        if (other.CompareTag("Puck"))
        {
           
                // Activar power-up al jugador
                //puck.lastPlayerTouched.ActivatePowerUp();

                
                int randomPower = 1; // Cambiar a Random.Range(1,4) 

                if (randomPower == 2)
                {
                    StartCoroutine(GiantPuck(other.transform));
                    Debug.Log("Puck Gigante");
                }else if(randomPower == 1)
                {
                    
                    GameObject newPuck = Instantiate(extraPuckPrefab, other.transform.position, Quaternion.identity);
                    Rigidbody rb = newPuck.GetComponent<Rigidbody>();
                    
                    Debug.Log("Nuevo puck creado");
                }
              
            

            // Desactivar la caja tras recogerla
            gameObject.SetActive(false);
        }
    }

    private IEnumerator GiantPuck(Transform puckTransform)
    {
        
        Debug.Log("Puck se hace gigante");
        Vector3 originalScale = puckTransform.localScale;
        puckTransform.localScale = originalScale * 1.5f;
        yield return new WaitForSeconds(1.5f);
        puckTransform.localScale = originalScale;
        Debug.Log("Puck vuelve a tamaño normal");
        
    }

}
