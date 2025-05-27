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
        if (insideValidZone==true)
        {
            transform.position = pos;
        }
        
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
