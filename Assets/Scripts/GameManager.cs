using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject puckPrefab;
    public int redScore = 0;
    public int blueScore = 0;
    public int scoreToWin = 5;

    public TextMeshProUGUI redScoreText;
    public TextMeshProUGUI blueScoreText;
    public TextMeshProUGUI winText;

    [Header("Menú y UI")]
    public GameObject menuCanvas;
    public GameObject gameObjects;
    public GameObject scoreCanvas;
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

    [Header("Contenedor de pucks")]
    public Transform puckContainer;

    [Header("Intervalo entre goles")]
    public float intervalGame = 5f; // Tiempo de pausa entre goles antes del respawn
    public GameObject shockwavePrefab;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (skipMenu)
        {
            StartGame();
        }
        else
        {
            if (menuCanvas != null) menuCanvas.SetActive(true);
            if (gameObjects != null) gameObjects.SetActive(false);
            if (scoreCanvas != null) scoreCanvas.SetActive(false);
            if (directionalLight != null) directionalLight.enabled = false;
        }
    }

    public void StartGame()
    {
        Debug.Log("StartGame");
        // Leer el estado actual de las opciones antes de cerrar el menú
        powerUpEnabled = powerUpOption != null && powerUpOption.GetState();
        obstacleEnabled = obstacleOption != null && obstacleOption.GetState();
        specialZoneEnabled = specialZoneOption != null && specialZoneOption.GetState();

        // Ocultar el menú
        if (menuCanvas != null) menuCanvas.SetActive(false);

        // Activar zona de juego
        if (gameObjects != null) gameObjects.SetActive(true);

        if (scoreCanvas != null) scoreCanvas.SetActive(true);

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

        // Ocultar el marcador al acabar la partida
        if (scoreCanvas != null)
            scoreCanvas.SetActive(false);

        // aqui se puede parar el juego o meter animaciones o lo que sea
        Time.timeScale = 0f;

        // Volver al menú
        if (menuCanvas != null) menuCanvas.SetActive(true);
        if (gameObjects != null) gameObjects.SetActive(false);

        // Apagar la luz del juego
        if (directionalLight != null) directionalLight.enabled = false;
    }
    
    public void RespawnPuck(float delay = 1f)
    {
        StartCoroutine(RespawnPuckCoroutine(delay));
    }

    private IEnumerator RespawnPuckCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Eliminar todos los pucks existentes en el contenedor
        if (puckContainer != null)
        {
            foreach (Transform child in puckContainer)
            {
                Destroy(child.gameObject);
            }
        }

        yield return new WaitForSeconds(intervalGame);

        GameObject newPuck = Instantiate(puckPrefab, new Vector3(0f, 0.5f, 0f), Quaternion.identity);
        if (puckContainer != null)
        {
            newPuck.transform.SetParent(puckContainer);
        }
    }

    public void TriggerShockwave(Vector3 position)
    {
        Instantiate(shockwavePrefab, position, Quaternion.Euler(90f, 0f, 0f));
    }
}
