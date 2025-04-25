using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int totalScore;
    public TMP_Text scoreDisplay;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        totalScore = 0;
        UpdateScoreUI();
    }

    public void AddScore(int baseScore)
    {
        int finalScore = baseScore * ComboManager.Instance.GetComboMultiplier();
        totalScore += finalScore;
        UpdateScoreUI();
    }

    public void SubmitScore(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Nama tidak boleh kosong!");
            return;
        }

        LeaderboardManager.Instance.SaveScore(playerName, totalScore);
        totalScore = 0;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreDisplay.text = $"Score: {totalScore}";
    }
}
