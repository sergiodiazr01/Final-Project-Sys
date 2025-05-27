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

        bool pu = powerUpOption?.GetState() ?? GameSettings.powerUpEnabled;
        bool ob = obstacleOption?.GetState() ?? GameSettings.obstacleEnabled;
        bool sz = specialZoneOption?.GetState() ?? GameSettings.specialZoneEnabled;

        // guarda de nuevo por si vino skipMenu
        GameSettings.powerUpEnabled = pu;
        GameSettings.obstacleEnabled = ob;
        GameSettings.specialZoneEnabled = sz;

        gameObjects?.SetActive(true);
        scoreCanvas?.SetActive(true);
        if (directionalLight) directionalLight.enabled = true;
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

    public void RespawnPuck(float delay)
    {
        StartCoroutine(RespawnCoroutine(delay));
    }

    private IEnumerator RespawnCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (puckContainer != null)
        {
            foreach (Transform c in puckContainer) Destroy(c.gameObject);
        }
        yield return new WaitForSeconds(delay);
        GameObject p = Instantiate(puckPrefab, Vector3.zero, Quaternion.identity);
        if (puckContainer != null) p.transform.SetParent(puckContainer);
    }

    public void TriggerShockwave(Vector3 pos, Color col)
    {
        GameObject go = Instantiate(shockwavePrefab, pos, Quaternion.Euler(90f, 0f, 0f));
        ShockWave sw = go.GetComponent<ShockWave>();
        if (sw != null) sw.SetColor(col);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scene");
    }
}
