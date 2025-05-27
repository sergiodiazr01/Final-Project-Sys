using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Hover Toggle Buttons")]
    public HoverToggleButton powerUpToggle;
    public HoverToggleButton obstacleToggle;
    public HoverToggleButton specialZoneToggle;

    [Header("Map Selector")]
    public MapSelectorManager mapSelector;

    [Header("Target Scene")]
    public string gameSceneName = "Estadio Scene";

    public void OnStartButtonPressed()
    {
        // Guardar estados de power-ups
        GameSettings.powerUpEnabled = powerUpToggle.GetState();
        GameSettings.obstacleEnabled = obstacleToggle.GetState();
        GameSettings.specialZoneEnabled = specialZoneToggle.GetState();
        Debug.Log(GameSettings.powerUpEnabled);
        Debug.Log(GameSettings.obstacleEnabled);
        Debug.Log(GameSettings.specialZoneEnabled);

        // Guardar mapa seleccionado
        if (mapSelector != null)
        {
            GameSettings.selectedMapIndex = mapSelector.GetCurrentIndex();
        }
        else
        {
            GameSettings.selectedMapIndex = 0;
            Debug.Log(GameSettings.selectedMapIndex);
        }

        // Cargar la escena de juego
        SceneManager.LoadScene(gameSceneName);
    }
}
