using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectorManager : MonoBehaviour
{
    public RawImage mapDisplay;
    public Texture[] mapTextures;
    private int index = 0;
    public int CurrentIndex => index;

    void Start()
    {
        if (mapTextures.Length > 0)
            //mapDisplay.texture = mapTextures[index];
            UpdateMap();
    }

    public void NextMap()
    {
        index = (index + 1) % mapTextures.Length;
        UpdateMap();
        GameSettings.MapIndex = index;
    }

    public void PreviousMap()
    {
        index = (index - 1 + mapTextures.Length) % mapTextures.Length;
        UpdateMap();
        GameSettings.MapIndex = index;
    }

    void UpdateMap()
    {
        mapDisplay.texture = mapTextures[index];
    }
}

