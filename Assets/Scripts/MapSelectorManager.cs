using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectorManager : MonoBehaviour
{
    public RawImage mapDisplay;
    public Texture[] mapTextures;
    private int index = 0;

    void Start()
    {
        if (mapTextures.Length > 0)
        {
            UpdateMap();
            index = 0;
        }
            //mapDisplay.texture = mapTextures[index];
    }

    public void NextMap()
    {
        index = (index + 1);
        UpdateMap();
    }

    public void PreviousMap()
    {
        index = (index - 1 + mapTextures.Length);
        UpdateMap();
    }

    void UpdateMap()
    {
        mapDisplay.texture = mapTextures[index];
        GameSettings.selectedMapIndex = index;
    }

    public int GetCurrentIndex()
    {
        return index;
    }
}

