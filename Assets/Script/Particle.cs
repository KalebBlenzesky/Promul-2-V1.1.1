using System.Collections;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public Bow bow;

    [Header("Prefabs to Spawn")]
    public GameObject prefab;

    [Header("Spawn Point & Target")]
    public Transform[] spawnPoints;
    public Transform targetPoint;

    [Header("Spawn Interval")]
    public float minSpawnInterval = 0.5f;
    public float maxSpawnInterval = 2f;

    [Header("Wave Frequency")]
    public float minWaveFrequency = 2f;
    public float maxWaveFrequency = 6f;

    [Header("Wave Amplitude")]
    public float minWaveAmplitude = 0.1f;
    public float maxWaveAmplitude = 0.5f;

    [Header("Move Speed")]
    public float minMoveSpeed = 1.5f;
    public float maxMoveSpeed = 4f;

    private Coroutine spawnCoroutine;
    private bool isSpawning = false;

    private void Update()
    {
        if (bow.isPulling && !isSpawning)
        {
            spawnCoroutine = StartCoroutine(SpawnRoutine());
            isSpawning = true;
        }
        else if (!bow.isPulling && isSpawning)
        {
            StopCoroutine(spawnCoroutine);
            isSpawning = false;
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float interval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(interval);

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject obj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

            ParticleMover movement = obj.GetComponent<ParticleMover>();
            if (movement != null)
            {
                float freq = Random.Range(minWaveFrequency, maxWaveFrequency);
                float amp = Random.Range(minWaveAmplitude, maxWaveAmplitude);
                float speed = Random.Range(minMoveSpeed, maxMoveSpeed);

                movement.SetTargetAndParams(targetPoint.position, freq, amp, speed);
            }
        }
    }
}
