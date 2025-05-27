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
            index = 0;
            UpdateMap();
        }
    }

    public void NextMap()
    {
        if (mapTextures.Length == 0) return;

        index = (index + 1) % mapTextures.Length;
        UpdateMap();
    }

    public void PreviousMap()
    {
        if (mapTextures.Length == 0) return;

        index = (index - 1 + mapTextures.Length) % mapTextures.Length;
        UpdateMap();
    }

    private void UpdateMap()
    {
        // Actualiza la imagen en el menú
        mapDisplay.texture = mapTextures[index];

        // Guarda la selección en la clase estática
        GameSettings.selectedMapIndex = index;
    }

    public int GetCurrentIndex()
    {
        return index;
    }
}
