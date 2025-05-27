using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int redScore = 0;
    public int blueScore = 0;
    public int scoreToWin = 3;

    public TextMeshProUGUI redScoreText;
    public TextMeshProUGUI blueScoreText;
    public TextMeshProUGUI winText;

    [Header("Menú y UI")]
    public GameObject gameObjects;
    public GameObject startButton;
    public Light directionalLight;

    [Header("Opciones de inicio")]
    public bool skipMenu = false;

    [Header("Selector de mapa")]
    public MapSelectorManager mapSelector;

    [Header("Opciones UI")]
    public HoverToggleButton powerUpOption;
    public HoverToggleButton obstacleOption;
    public HoverToggleButton specialZoneOption;

    [Header("Estado de opciones activadas")]
    public bool powerUpEnabled = false;
    public bool obstacleEnabled = false;
    public bool specialZoneEnabled = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (GameSettings.powerUpEnabled || GameSettings.obstacleEnabled || GameSettings.specialZoneEnabled)
        {
            powerUpEnabled = GameSettings.powerUpEnabled;
            obstacleEnabled = GameSettings.obstacleEnabled;
            specialZoneEnabled = GameSettings.specialZoneEnabled;
            StartGame();
        }
        else if (skipMenu)
        {
<<<<<<< Updated upstream
            if (menuCanvas != null) menuCanvas.SetActive(true);
            if (gameObjects != null) gameObjects.SetActive(false);
            if (directionalLight != null) directionalLight.enabled = false;
=======
            // Opción de debug: saltar siempre sin menú
            StartGame();
>>>>>>> Stashed changes
        }
    }

    public void StartGame()
    {
        Debug.Log("StartGame");
        // Leer el estado actual de las opciones antes de cerrar el menú
        powerUpEnabled = powerUpOption != null && powerUpOption.GetState();
        obstacleEnabled = obstacleOption != null && obstacleOption.GetState();
        specialZoneEnabled = specialZoneOption != null && specialZoneOption.GetState();

        // Activar zona de juego
        if (gameObjects != null) gameObjects.SetActive(true);

        // Activar la luz de juego
        if (directionalLight != null) directionalLight.enabled = false;
    }

    public void GoalScored(PlayerTeam scoringTeam)
    {
        if (scoringTeam == PlayerTeam.Red)
            redScore++;
        else
            blueScore++;

        UpdateUI();

        if (redScore >= scoreToWin)
            ShowWin("¡Gana el equipo ROJO!");
        else if (blueScore >= scoreToWin)
            ShowWin("¡Gana el equipo AZUL!");
    }

    void UpdateUI()
    {
        redScoreText.text = redScore.ToString();
        blueScoreText.text = blueScore.ToString();
    }

    void ShowWin(string message)
    {
        winText.gameObject.SetActive(true);
        winText.text = message;

<<<<<<< Updated upstream
        // aqui se puede parar el juego o meter animaciones o lo que sea
        Time.timeScale = 0f;

        // Volver al menú
        if (menuCanvas != null) menuCanvas.SetActive(true);
        if (gameObjects != null) gameObjects.SetActive(false);

        // Apagar la luz del juego
        if (directionalLight != null) directionalLight.enabled = false;
=======
        // Desactivar marcador
        if (scoreCanvas != null)
            scoreCanvas.SetActive(false);

        // 3) Parar todo bajo gameObjects
        if (gameObjects != null)
        {
            // Desactiva el objeto padre
            gameObjects.SetActive(false);

            // (Opcional) Asegura que cada hijo queda desactivado
            foreach (Transform child in gameObjects.transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        // 4) Apagar la luz de juego
        if (directionalLight != null)
            directionalLight.enabled = false;

        //Time.timeScale = 0f;
>>>>>>> Stashed changes
    }
}
