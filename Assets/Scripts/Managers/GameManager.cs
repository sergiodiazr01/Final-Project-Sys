using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

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

    [Header("Mapas de estadio")]
    public Renderer mapRenderer;
    public Material[] mapMaterials;

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
    public float intervalGame = 5f; //tiempo de pausa entre goles antes del respawn del puck
    public GameObject shockwavePrefab;

    private AudioSource audioSource;
    public AudioClip countdownSound;
    public AudioClip WinSound;

    [Header("Pantallas de victoria")]
    public GameObject redVictoryImage;
    public GameObject blueVictoryImage;

    [Header("Spawners")]
    public GameObject powerUpSpawner;
    public GameObject specialZoneSpawner;
    public GameObject obstacleSpawner;

    [Header("Mapa")]
    private int selectedMapIndex = 0;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (skipMenu)
        {
            StartGame();
        }

        if (!powerUpEnabled && powerUpSpawner != null)
            powerUpSpawner.gameObject.SetActive(false);

        if (!specialZoneEnabled && specialZoneSpawner != null)
            specialZoneSpawner.gameObject.SetActive(false);

        if (!obstacleEnabled && obstacleSpawner != null)
            obstacleSpawner.gameObject.SetActive(false);

        
        //activar UI de juego
        scoreCanvas.SetActive(true);
        gameObjects.SetActive(true);
        audioSource = GetComponent<AudioSource>();

        //else
        //{
        //    if (menuCanvas != null) menuCanvas.SetActive(true);
        //    if (gameObjects != null) gameObjects.SetActive(false);
        //    if (scoreCanvas != null) scoreCanvas.SetActive(false);
        //    if (directionalLight != null) directionalLight.enabled = false;
        //}
    }

    public void StartGame()
    {
        SceneManager.LoadScene("EstadioScene");
    }

    public void GoalScored(PlayerTeam scoringTeam)
    {

        if (scoringTeam == PlayerTeam.Red)
            redScore++;
        else
            blueScore++;

        UpdateUI();
        if (redScore < scoreToWin || blueScore < scoreToWin)
        {
           StartCoroutine(PlayCountdownSoundDelayed(1f));
        }

        if (redScore >= scoreToWin)
            ShowWin(PlayerTeam.Red);
        else if (blueScore >= scoreToWin)
            ShowWin(PlayerTeam.Blue);
    }

    void UpdateUI()
    {
        redScoreText.text = redScore.ToString();
        blueScoreText.text = blueScore.ToString();
    }
    void ShowWin(PlayerTeam winner)
    {

        if (winner == PlayerTeam.Red && redVictoryImage != null)
        {
            if (WinSound != null)
            {
                audioSource.PlayOneShot(WinSound);
            }
            redVictoryImage.SetActive(true);
        }
        else if (winner == PlayerTeam.Blue && blueVictoryImage != null)
        {
            if (WinSound != null)
            {
                audioSource.PlayOneShot(WinSound);
            }

            blueVictoryImage.SetActive(true);
        }


        if (scoreCanvas != null)
            scoreCanvas.SetActive(false);

        //desactivar gameobjects 
        if (gameObjects != null)
        {
            gameObjects.SetActive(false);
        }
        scoreCanvas?.SetActive(false);
        if (directionalLight) directionalLight.enabled = false;
        //Time.timeScale = 0f;

        
        if (directionalLight != null)
            directionalLight.enabled = false;
    }


    public void RespawnPuck(float delay = 1f)
    {
        StartCoroutine(RespawnPuckCoroutine(delay));
    }

    private IEnumerator RespawnPuckCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        //eliminar todos los pucks del container
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

    public void TriggerShockwave(Vector3 position, Color color)
    {
        GameObject waveObject = Instantiate(shockwavePrefab, position, Quaternion.Euler(90f, 0f, 0f));
        Debug.Log("a");
        ShockWave shockWave = waveObject.GetComponent<ShockWave>();
        Debug.Log("aa");
        shockWave.SetColor(color);
        Debug.Log("aaa");
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;  
        SceneManager.LoadScene("Menu");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "EstadioScene")
            return;

        //ajustes
        selectedMapIndex = GameSettings.MapIndex;
        powerUpEnabled = GameSettings.PowerUpEnabled;
        specialZoneEnabled = GameSettings.SpecialZoneEnabled;
        obstacleEnabled = GameSettings.ObstacleEnabled;

        //activar estadio
        mapRenderer.material = mapMaterials[selectedMapIndex];

        //activar/desactivar spawners
        if (powerUpSpawner != null) powerUpSpawner.gameObject.SetActive(powerUpEnabled);
        if (specialZoneSpawner != null) specialZoneSpawner.gameObject.SetActive(specialZoneEnabled);
        if (obstacleSpawner != null) obstacleSpawner.gameObject.SetActive(obstacleEnabled);

        //reset de marcadores y UI
        redScore = blueScore = 0;
        UpdateUI();
        scoreCanvas?.SetActive(true);
        gameObjects?.SetActive(true);
        directionalLight?.gameObject.SetActive(true);
    }
    
    private IEnumerator PlayCountdownSoundDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (countdownSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(countdownSound);
        }
    }
}