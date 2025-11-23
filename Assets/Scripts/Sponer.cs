using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sponer : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float WaveInterval = 30f;
    [SerializeField] private int enemiesPerWave = 10;

    private int currentWave = 0;
    private const int MAX_WAVES = 3;
    public static int monstersAlive = 0;
    public static bool allWavesFinished = false;

    private void Start()
    {
        mapManager = FindObjectOfType<MapManager>();
        StartCoroutine(ManageWaves());
    }

    IEnumerator ManageWaves()
    {
        while (currentWave < MAX_WAVES)
        {
            currentWave++;
            Debug.Log($"웨이브 {currentWave} 시작");

            yield return StartCoroutine(SpawnEnemiesSquentially());

            if (currentWave == MAX_WAVES)
            {
                allWavesFinished = true;
                yield break;
            }

            yield return new WaitForSeconds(WaveInterval);
        }

        allWavesFinished = true;
    }

    IEnumerator SpawnEnemiesSquentially()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        { 
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            Sponer.monstersAlive++;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
