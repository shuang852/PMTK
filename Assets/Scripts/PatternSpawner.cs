using System;
using System.Collections.Generic;
using System.Timers;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class PatternSpawner : MonoBehaviour
{
    [Serializable]
    private struct PatternSegment
    {
        public GameObject prefab;
        public float startTimeFrame;
        public float endTimeFrame;
    }
    
    [SerializeField] private float TimeBetweenSpawnMax;
    [SerializeField] private float TimeBetweenSpawnMin;
    [SerializeField] private float timeBetweenSpawnDecreaseInterval = 3f;
    [SerializeField] private float timeBetweenSpawnDecrease;
    [SerializeField] private float timeBetweenSpawnDecreaseMax = 0;
    [SerializeField] private float startDelay = 3f;
    [SerializeField] private List<GameObject> patternPrefabs;
    [SerializeField] private List<PatternSegment> patternSegments; 

    private GameManager gameManager;
    private float timer = 0;
    private float startTimer;
    private float currentIncreaseInterval;
    private float startTimeBetweenSpawnMax;
    private float startTimeBetweenSpawnMin;
    private float totalIncrease;
    private float timeFrame;
    
    public HashSet<GameObject> SpawnedPatternObjects { get; } = new HashSet<GameObject>();
    
    public static PatternSpawner Instance { get; private set; }

    private int lastIndex;

    void Start()
    {
        Instance = this;
        gameManager = FindObjectOfType<GameManager>();
        timer = TimeBetweenSpawnMax;

        startTimeBetweenSpawnMin = TimeBetweenSpawnMin;
        startTimeBetweenSpawnMax = TimeBetweenSpawnMax;
        lastIndex = -1;
    }

    public void Reset()
    {
        startTimer = 0;
        timer = 0;
        TimeBetweenSpawnMin = startTimeBetweenSpawnMin;
        TimeBetweenSpawnMax = startTimeBetweenSpawnMax;
        totalIncrease = 0;
        timeFrame = 0;
        lastIndex = -1;
    }

    public void ClearObjects()
    {
        foreach (GameObject go in SpawnedPatternObjects)
        {
            Destroy(go);
        }
    }

    private void Update()
    {
        if (gameManager.GameActive && startTimer > startDelay)
        {
            if (timer <= 0)
            {
                SpawnRandomPattern();
                timer = Random.Range(TimeBetweenSpawnMin, TimeBetweenSpawnMax);
            }

            timer -= Time.deltaTime;
        }

        if (totalIncrease < timeBetweenSpawnDecreaseMax && currentIncreaseInterval >= timeBetweenSpawnDecreaseInterval)
        {
            TimeBetweenSpawnMin -= timeBetweenSpawnDecrease;
            TimeBetweenSpawnMax -= timeBetweenSpawnDecrease;
            totalIncrease += timeBetweenSpawnDecrease;
            currentIncreaseInterval = 0;
        }
        
        if (GameManager.Instance.GameActive)
        {
            startTimer += Time.deltaTime;
            currentIncreaseInterval += Time.deltaTime;
            timeFrame += Time.deltaTime;
        }
    }

    private void SpawnRandomPattern()
    {
        GameObject patternObject = null;
        HashSet<int> visited = new HashSet<int>();

        do
        {
            int index = Random.Range(0, patternSegments.Count);
            var segment = patternSegments[index];

            if (index != lastIndex && !visited.Contains(index) && timeFrame >= segment.startTimeFrame && timeFrame <= segment.endTimeFrame)
            {
                patternObject = segment.prefab;
                lastIndex = index;
                break;
            }
            visited.Add(index);
        } while (visited.Count < patternSegments.Count);

        if (patternObject != null)
        {
            Vector2 position = new Vector2(
                gameManager.ViewportRightSide * 3,
                0
            );

            GameObject go = Instantiate(patternObject, position, quaternion.identity, transform);
            SpawnedPatternObjects.Add(go);
        }
    }

    public void RemoveSpawnPatternObject(GameObject go)
    {
        SpawnedPatternObjects.Remove(go);
    }
}
