using System.Collections;
using Managment;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Item Pools")] [Tooltip("Items the wife needs to collect (11% chance)")] [SerializeField]
    private GameObject[] wifeObjectives;

    [Tooltip("Items the husband needs to collect (33% chance)")] [SerializeField]
    private GameObject[] husbandObjectives;

    [Tooltip("Regular power-ups and weapons (55% chance)")] [SerializeField]
    private GameObject[] regularItems;

    [Header("Time Settings (Seconds)")] [SerializeField]
    private float minSpawnDelay = 3f;

    [SerializeField] private float maxSpawnDelay = 7f;

    // [Header("Position Settings")] [SerializeField]
    // private float spawnHeight = 10f; //Was used when items fell from the sky

    private const float SpawnRotationY = 90f;

    [Tooltip("Spawn boundaries on the X axis")] [SerializeField]
    private float minX = -34f;

    [SerializeField] private float maxX = 12f;

    [Tooltip("Spawn boundaries on the Z axis (depth)")] [SerializeField]
    private float minZ = -29f;

    [SerializeField] private float maxZ = 33f;

    // private bool inGamePlayMode = false;

    private const int WifeObjectiveRoll = 1;
    private const int ObjectivesRoll = 20;
    private const int HusbandObjectiveRoll = (int)(ObjectivesRoll*0.45);


    private void Start()
    {
        // StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(waitTime);
            Spawn();
        }
    }

    private void Spawn(int roll = 0)
    {
        GameObject prefabToSpawn = GetRandomItemByProbability(roll);

        if (prefabToSpawn)
        {
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            Vector3 spawnPosition = new Vector3(randomX, 0, randomZ);
            Quaternion spawnRotation = Quaternion.Euler(0f, SpawnRotationY, 0f);

            Instantiate(prefabToSpawn, spawnPosition, spawnRotation);
        }
    }

    private GameObject GetRandomItemByProbability(int roll)
    {
        if (roll == 0)
            roll = Random.Range(1, ObjectivesRoll);
        GameObject[] selectedPool = null;

        if (roll <= WifeObjectiveRoll)
        {
            selectedPool = wifeObjectives;
        }
        else if (roll <= HusbandObjectiveRoll)
        {
            selectedPool = husbandObjectives;
        }
        else
        {
            selectedPool = regularItems;
        }

        // Ensure the selected pool is not empty to avoid errors
        if (selectedPool != null && selectedPool.Length > 0)
        {
            int randomIndex = Random.Range(0, selectedPool.Length);
            return selectedPool[randomIndex];
        }

        return null;
    }

    private void OnEnable()
    {
        GameManager.OnGameStarted += TriggerGameplay;
        GameManager.OnGameOver += TriggerGameOver;
    }

    private void TriggerGameplay()
    {
        // inGamePlayMode = true;
        Spawn(HusbandObjectiveRoll);
        StartCoroutine(SpawnRoutine());
    }

    private void TriggerGameOver()
    {
        // inGamePlayMode = false;
        StopCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        GameManager.OnGameStarted -= TriggerGameplay;
        GameManager.OnGameOver -= TriggerGameOver;
    }
}