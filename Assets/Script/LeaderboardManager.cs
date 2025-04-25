using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int score;
}

[System.Serializable]
public class PlayerDataList
{
    public List<PlayerData> players = new List<PlayerData>();
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    public TMP_Text BestLeaderBoardName;
    public TMP_Text BestLeaderBoardScore;

    private PlayerDataList playerDataList = new PlayerDataList();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadData();
        }
    }

    private void Start()
    {
        UpdateLeaderboardUI();
    }

    public void SaveScore(string playerName, int score)
    {
        playerDataList.players.Add(new PlayerData { playerName = playerName, score = score });
        SaveData();
        UpdateLeaderboardUI();
    }

    private void SaveData()
    {
        string json = JsonUtility.ToJson(playerDataList);
        PlayerPrefs.SetString("PlayerScores", json);
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        string json = PlayerPrefs.GetString("PlayerScores", "{}");
        playerDataList = JsonUtility.FromJson<PlayerDataList>(json);
    }

    private void UpdateLeaderboardUI()
    {
        if(BestLeaderBoardName != null && BestLeaderBoardScore != null)
        {
            BestLeaderBoardName.text = "";
            BestLeaderBoardScore.text = "";

            var sortedPlayers = playerDataList.players.OrderByDescending(p => p.score).ToList();

            foreach (var player in sortedPlayers)
            {
                BestLeaderBoardName.text += $"{player.playerName}\n";
                BestLeaderBoardScore.text += $"{player.score}\n";
            }
        }
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteKey("PlayerScores");
        playerDataList.players.Clear();
        PlayerPrefs.Save();
        UpdateLeaderboardUI();
        Debug.Log("Semua data telah dihapus!");
    }
}
