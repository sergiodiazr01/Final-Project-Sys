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

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
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

        // aqui se puede parar el juego o meter animaciones o lo que sea
        Time.timeScale = 0f;
    }
}
