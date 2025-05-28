using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public HoverToggleButton powerUpOption;
    public HoverToggleButton obstacleOption;
    public HoverToggleButton specialZoneOption;
    public MapSelectorManager mapSelector;
    public Button startButton;

    void Start()
    {
        //startButton.onClick.AddListener(OnStartClicked);
    }

    void OnStartClicked()
    {
        bool pu = powerUpOption.GetState();
        bool obs = obstacleOption.GetState();
        bool sz = specialZoneOption.GetState();
        int mapIdx = mapSelector.CurrentIndex;

        //GameManager.instance.StartGame();
    }
}
