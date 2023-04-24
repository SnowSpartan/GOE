using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private float spawnRange = 200;
    public GameObject enemyPrefab;
    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemies();
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(0, spawnRange);
        float spawnPosY = Random.Range(0, spawnRange);
        Vector3 randomPos = new Vector3(spawnPosX, 5, spawnPosY);
        return randomPos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < 5; i++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
        }
    }
}
