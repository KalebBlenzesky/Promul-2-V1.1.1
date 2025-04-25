using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Target : MonoBehaviour
{
    public int scoreValue = 10;
    public bool isTarget;
    private ScoreManager scoreManager;

    void Start()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Arrow"))
        {
            if (isTarget)
            {
                scoreManager.AddScore(scoreValue);
            }
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
