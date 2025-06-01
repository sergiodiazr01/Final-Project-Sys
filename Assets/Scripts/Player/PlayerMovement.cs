using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public Quaternion q;
    public bool manual;
    public bool insideValidZone;
    private PlayerController playerController;

    [Header("Restriccion de mapa")]
    public Vector2 limitX = new Vector2(-44f, 44f);
    public Vector2 limitZ = new Vector2(-24f, 24f);
    public bool isMenu = false;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    /*void Update()
    {
        if (playerController == null) return;

        Vector3 pos = transform.position;

        if (playerController.team == PlayerTeam.Red)
        {
            pos.x = Mathf.Clamp(pos.x, 4f, 50f);
            pos.z = Mathf.Clamp(pos.z, 18f, 71f);
        }
        else if (playerController.team == PlayerTeam.Blue)
        {
            pos.x = Mathf.Clamp(pos.x, -40f, 4f);
            pos.z = Mathf.Clamp(pos.z, 18f, 71f);
        }

        transform.position = pos;
    }*/


    public void SetPosition(Vector3 pos)
    {
        if (isMenu)
        {
            transform.position = pos;
            return;
        }
        if (true)
        {
        //comprobamos si el tracking original esta dentro de los limites
        bool withinX = (pos.x >= limitX.x && pos.x <= limitX.y);
        bool withinZ = (pos.z >= limitZ.x && pos.z <= limitZ.y);
        insideValidZone = withinX && withinZ;

        //clamp de la posicion para que el player no salga del campo
        float clampedX = Mathf.Clamp(pos.x, limitX.x, limitX.y);
        float clampedZ = Mathf.Clamp(pos.z, limitZ.x, limitZ.y);

        //aplicamos la posicion final (solo x,z)
        transform.position = new Vector3(clampedX, transform.position.y, clampedZ);
        }

    }

    private void LateUpdate()
    {
        if (isMenu)
        {
            return;
        }
        Vector3 position = transform.position;

        //clamp global
        position.x = Mathf.Clamp(position.x, limitX.x, limitX.y);
        position.z = Mathf.Clamp(position.z, limitZ.x, limitZ.y);

        //clamp por equipo (mitad de campo)
        if (playerController != null)
        {
            if (playerController.team == PlayerTeam.Red)
            {
                //rojo no puede pasar a izquierda (x < 0)
                position.x = Mathf.Clamp(position.x, 0f, limitX.y);
            }
            else 
            {
                //azul no puede pasar a derecha (x > 0)
                position.x = Mathf.Clamp(position.x, limitX.x, 0f);
            }
        }

        transform.position = position;
    }

    // Getter for position
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    // Setter for rotation
    public void SetRotation(Quaternion rot)
    {
        transform.rotation = rot;
    }

    // Getter for rotation
    public Quaternion GetRotation()
    {
        return transform.rotation;
    }
}
