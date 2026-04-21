using System.Collections.Generic;
using LaneObjects;
using Managment;
using Scriptable_Objects;
using UnityEngine;

namespace Management
{
    public class LaneManager : MonoBehaviour
    {
        [Header("Lane Blueprint")]
        [Tooltip("Object that inherits from MovableLaneObject")]
        [SerializeField] private MovableLaneObject lanePrefab; 
    
        [Tooltip("Scriptable Object that inherits from MovableLaneObject and corresponds to lanePrefab")]
        [SerializeField] private List<LaneObjectData> objectsData;

        [Header("Lane Settings")]
        [SerializeField] private float laneSpeed;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform despawnPoint;
    
        [SerializeField] private bool forward;
    
        [SerializeField] private float minSpawnDelay;
        [SerializeField] private float maxSpawnDelay;

        private float _spawnTimer;

        void Start() => SetNextSpawnTime();
        
        // private void OnEnable()
        // {
        //     GameManager.OnGameStarted += BeginSpawning;
        // }
        //
        //
        // private void OnDisable()
        // {
        //     GameManager.OnGameStarted -= BeginSpawning;
        // }
        
        // private void BeginSpawning()
        // {
        //     throw new System.NotImplementedException();
        // }

        void Update()
        {
            if (GameManager.Instance.currentState != GameManager.GameState.Gameplay) return;

            if (Time.time > _spawnTimer)
            {
                SpawnObject();
                SetNextSpawnTime();
            }
        }

        private void SpawnObject()
        {
            if (objectsData.Count == 0 || !lanePrefab) return;

            LaneObjectData randomData = objectsData[Random.Range(0, objectsData.Count)];
        
            MovableLaneObject newObj = lanePrefab.PullFromPool();
        
            newObj.transform.position = spawnPoint.position;
            int direction = forward ? 1 : -1;
        
            newObj.SetupFromData(randomData, laneSpeed, direction, despawnPoint);
        }

        private void SetNextSpawnTime()
        {
            _spawnTimer = Random.Range(minSpawnDelay, maxSpawnDelay) + Time.time;
        }
    }
}