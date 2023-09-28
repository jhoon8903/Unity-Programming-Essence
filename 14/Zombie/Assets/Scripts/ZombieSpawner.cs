using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public Zombie zombiePrefab;
    public ZombieData[] zombieDatas;
    public Transform[] spawnPoints;
    private List<Zombie> zombies = new List<Zombie>();
    private int wave = 0;

    private void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            return;
        }

        if (zombies.Count <= 0)
        {
            SpawnWave();
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        UIManager.instance.UpdateWaveText(wave, zombies.Count);
    }

    private void SpawnWave()
    {
        wave++;
        var spawnCount = Mathf.RoundToInt(wave * 1.5f);
        for (var i = 0; i < spawnCount; i++)
        {
            CreateZombie();
        }
    }

    private void CreateZombie()
    {
        var zombieData = zombieDatas[Random.Range(0, zombieDatas.Length)];
        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        var zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);
        zombie.Setup(zombieData);
        zombies.Add(zombie);
        zombie.OnDeath += () => zombies.Remove(zombie);
        zombie.OnDeath += () => Destroy(zombie.gameObject, 10f);
        zombie.OnDeath += () => GameManager.instance.AddScore(100);
    }
}
