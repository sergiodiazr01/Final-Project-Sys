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
    public float speedZoneMultiplier = 2f; // Multiplicador de velocidad en la zona de velocidad

    private Rigidbody rb;
    public float hitforce = 100f; // Fuerza de golpeo del puck
    void Start()
    {
        puckRenderer.material.color = defaultColor;
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {   
        if (collision.gameObject.CompareTag("PlayerRed") || collision.gameObject.CompareTag("PlayerBlue"))
        {
            lastPlayerTouched = collision.gameObject.GetComponent<PlayerController>();

            Vector3 direction = (rb.position - collision.contacts[0].point).normalized;
            //float impactSpeed = collision.relativeVelocity.magnitude;
            //float scaledForce = impactSpeed * hitforce;
            //rb.AddForce(direction * scaledForce, ForceMode.Impulse);
            rb.AddForce(direction * hitforce, ForceMode.Impulse);


            if (collision.gameObject.CompareTag("PlayerRed"))
            {
                Debug.Log("a");
                puckRenderer.material.color = redPlayerColor;
            }
            else if (collision.gameObject.CompareTag("PlayerBlue"))
            {
                puckRenderer.material.color = bluePlayerColor;
            }
        }

        if(collision.gameObject.CompareTag("SpeedZone")) //podemos poner tags distintos a cada zona o 
        // hacer como en los power ups de poner numeros random del 1 al que queramos y segun el que toque que sea una zona
        {
            //aumentar la velocidad del puck
            speed *= speedZoneMultiplier;
            Debug.Log("Speed Zone: " + speed);
           
        }
        {
            
        }
    }
}
