using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public HoverToggleButton powerUpToggle;
    public HoverToggleButton obstacleToggle;
    public HoverToggleButton specialZoneToggle;
    public MapSelectorManager mapSelector;
    public string gameSceneName = "EstadioScene";

    public void OnStartButtonPressed()
    {
        GameSettings.powerUpEnabled = powerUpToggle.GetState();
        GameSettings.obstacleEnabled = obstacleToggle.GetState();
        GameSettings.specialZoneEnabled = specialZoneToggle.GetState();
        GameSettings.selectedMapIndex = mapSelector.GetCurrentIndex();

        SceneManager.LoadScene(gameSceneName);
    }
}
