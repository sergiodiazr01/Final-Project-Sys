using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHoverArrow : MonoBehaviour
{
    public enum Direction { Left, Right }
    public Direction arrowDirection;

    public float hoverTime = 2f;
    private float timer = 0f;
    private bool isHovering = false;

    private MapSelectorManager selector;

    void Start()
    {
        selector = FindObjectOfType<MapSelectorManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isHovering = true;
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
                if (arrowDirection == Direction.Left)
                    selector.PreviousMap();
                else
                    selector.NextMap();

                timer = 0f;
                isHovering = false;
            }
        }
    }
}
