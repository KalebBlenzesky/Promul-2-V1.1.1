using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [Header("Prefabs to Spawn")]
    public List<GameObject> prefabs;

    [Header("Waypoints Groups")]
    public List<WaypointGroup> waypointGroups;

    [Header("Settings")]
    public float spawnDelay = 2f;
    public int maxSpawnedObjects = 5;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<WaypointGroup> usedWaypointGroups = new List<WaypointGroup>();

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);

            spawnedObjects.RemoveAll(item => item == null);
            usedWaypointGroups.RemoveAll(group => group == null);

            if (spawnedObjects.Count < maxSpawnedObjects)
            {
                SpawnRandomObject();
            }
        }
    }

    private void SpawnRandomObject()
    {
        if (prefabs.Count == 0 || waypointGroups.Count == 0)
        {
            Debug.LogWarning("Prefab list atau WaypointGroup list kosong!");
            return;
        }

        List<WaypointGroup> availableGroups = new List<WaypointGroup>();
        foreach (var group in waypointGroups)
        {
            if (!usedWaypointGroups.Contains(group))
            {
                availableGroups.Add(group);
            }
        }

        if (availableGroups.Count == 0)
        {
            Debug.LogWarning("Tidak ada lagi WaypointGroup yang tersedia!");
            return;
        }

        GameObject prefabToSpawn = prefabs[Random.Range(0, prefabs.Count)];
        WaypointGroup selectedGroup = availableGroups[Random.Range(0, availableGroups.Count)];

        GameObject spawned = Instantiate(prefabToSpawn, selectedGroup.waypoints[0].position, Quaternion.identity);

        Target mover = spawned.GetComponent<Target>();
        if (mover != null)
        {
            mover.SetWaypoints(selectedGroup.waypoints);
        }

        spawnedObjects.Add(spawned);
        usedWaypointGroups.Add(selectedGroup);
    }

    private void OnDrawGizmos()
    {
        if (waypointGroups == null)
            return;

        Gizmos.color = Color.yellow;

        foreach (var group in waypointGroups)
        {
            if (group == null || group.waypoints == null || group.waypoints.Count == 0)
                continue;

            for (int i = 0; i < group.waypoints.Count; i++)
            {
                if (group.waypoints[i] == null)
                    continue;

                // Gambar bola kecil di setiap waypoint
                Gizmos.DrawSphere(group.waypoints[i].position, 0.2f);

                // Gambar garis ke waypoint berikutnya (jika ada)
                if (i < group.waypoints.Count - 1 && group.waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(group.waypoints[i].position, group.waypoints[i + 1].position);
                }
            }
        }
    }
}

[System.Serializable]
public class WaypointGroup
{
    public List<Transform> waypoints;
}
