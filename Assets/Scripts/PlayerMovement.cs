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

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if ((playerController.team == PlayerTeam.Red && other.CompareTag("RedMitad")) ||
            (playerController.team == PlayerTeam.Blue && other.CompareTag("BlueMitad")))
        {
            insideValidZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((playerController.team == PlayerTeam.Red && other.CompareTag("RedMitad")) ||
            (playerController.team == PlayerTeam.Blue && other.CompareTag("BlueMitad")))
        {
            insideValidZone = false;
        }
    }
    // Setter for position
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
