using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Target : MonoBehaviour
{
    public int scoreValue = 10;
    public bool isTarget;
    private ScoreManager scoreManager;

    public float moveSpeed = 3f;
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = 0;
    void Start()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        MoveToWaypoint();
    }

    public void SetWaypoints(List<Transform> waypointList)
    {
        waypoints = waypointList;
        currentWaypointIndex = 0;
    }

    private void MoveToWaypoint()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                currentWaypointIndex = 0; // Loop kembali ke waypoint pertama
            }
        }
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
