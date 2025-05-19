using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goal : MonoBehaviour
{
     public PlayerTeam goalTeam;  // El equipo que recibe el gol si entra aquí
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Puck"))
        {
            PlayerTeam scoringTeam = (goalTeam == PlayerTeam.Red) ? PlayerTeam.Blue : PlayerTeam.Red;
            GameManager.instance.GoalScored(scoringTeam);

            
           
            
            Rigidbody rb = other.GetComponentInParent<Rigidbody>();//In parent porque estamos en la mesh, no en el puck prefab
            rb.velocity = Vector3.zero; // Detener el puck
            rb.angularVelocity = Vector3.zero; // Detener el puck
            rb.transform.position = new Vector3(0, 0.5f, 0); // reposicionamiento al centro
           
        }
    }
}

