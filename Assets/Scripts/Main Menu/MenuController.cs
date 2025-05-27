using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    public SceneController _sceneController;

    // Dipanggil kalau tombol "Play" diklik
    public void PlayGame()
    {
        // Ganti "NamaSceneGame" dengan nama scene game kamu
        _sceneController.LoadScene("Ayii");
    }

    // Dipanggil kalau tombol "Exit" diklik
    public void ExitGame()
    {
        // Ini akan keluar dari aplikasi (kalau dijalankan di build, bukan di editor)
        Application.Quit();
        
        // Untuk tes di editor bisa tambahkan:
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
