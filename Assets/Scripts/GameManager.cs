using UnityEngine;
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

    [Header("Partida")]
    public GameObject gameObjects;
    public GameObject scoreCanvas;
    public Light directionalLight;

    [Header("Opciones UI")]
    public HoverToggleButton powerUpOption;
    public HoverToggleButton obstacleOption;
    public HoverToggleButton specialZoneOption;

    private bool powerUpEnabled = false;
    private bool obstacleEnabled = false;
    private bool specialZoneEnabled = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Si venimos del menú con opciones, las aplicamos
        if (GameSettings.powerUpEnabled || GameSettings.obstacleEnabled || GameSettings.specialZoneEnabled)
        {
            powerUpEnabled = GameSettings.powerUpEnabled;
            obstacleEnabled = GameSettings.obstacleEnabled;
            specialZoneEnabled = GameSettings.specialZoneEnabled;
            StartGame();
        }
        else
        {
            // Esperamos a que alguien llame a StartGame()
            if (gameObjects != null) gameObjects.SetActive(false);
            if (scoreCanvas != null) scoreCanvas.SetActive(false);
            if (directionalLight != null) directionalLight.enabled = false;
        }
    }

    public void StartGame()
    {
        Debug.Log("StartGame");

        // Leer toggles si no venimos del menú
        powerUpEnabled = powerUpOption != null && powerUpOption.GetState();
        obstacleEnabled = obstacleOption != null && obstacleOption.GetState();
        specialZoneEnabled = specialZoneOption != null && specialZoneOption.GetState();

        // Activar elementos de juego
        if (gameObjects != null) gameObjects.SetActive(true);
        if (scoreCanvas != null) scoreCanvas.SetActive(true);
        if (directionalLight != null) directionalLight.enabled = true;
    }

    public void GoalScored(PlayerTeam scoringTeam)
    {
        if (scoringTeam == PlayerTeam.Red) redScore++;
        else blueScore++;

        UpdateUI();

        if (redScore >= scoreToWin) ShowWin("¡Gana el equipo ROJO!");
        if (blueScore >= scoreToWin) ShowWin("¡Gana el equipo AZUL!");
    }

    private void UpdateUI()
    {
        redScoreText.text = redScore.ToString();
        blueScoreText.text = blueScore.ToString();
    }

    private void ShowWin(string message)
    {
        if (winText != null)
        {
            winText.gameObject.SetActive(true);
            winText.text = message;
        }

        // Parar la partida
        if (gameObjects != null) gameObjects.SetActive(false);
        if (scoreCanvas != null) scoreCanvas.SetActive(false);
        if (directionalLight != null) directionalLight.enabled = false;
    }
}
