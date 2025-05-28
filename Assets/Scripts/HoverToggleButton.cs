using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.UI;

public class HoverToggleButton : MonoBehaviour
{
    public Sprite onSprite;
    public Sprite offSprite;
    public float hoverTime = 2f;

    private bool isOn = false;
    private float timer = 0f;
    private bool isHovering = false;
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        UpdateVisual();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isHovering = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isHovering = false;
            timer = 0f;
        }
    }

    void Update()
    {
        if (isHovering)
        {
            timer += Time.deltaTime;
            if (timer >= hoverTime)
            {
                ToggleState();
                timer = 0f;
                isHovering = false; // evita repeticiones si el jugador se queda encima
            }
        }
    }

    void ToggleState()
    {
        isOn = !isOn;
        UpdateVisual();
        Debug.Log(gameObject.name + " toggled to " + (isOn ? "ON" : "OFF"));
        if (gameObject.name.Contains("PowerUps"))
            GameSettings.PowerUpEnabled = isOn;
        else if (gameObject.name.Contains("Obstacles"))
            GameSettings.ObstacleEnabled = isOn;
        else if (gameObject.name.Contains("SpecialZone"))
            GameSettings.SpecialZoneEnabled = isOn;
    }

    void UpdateVisual()
    {
        if (image != null)
            image.sprite = isOn ? onSprite : offSprite;
    }

    public bool GetState()
    {
        return isOn;
    }
}