using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnTime
{
    public float MinTime;
    public float MaxTime;
}

public class SpawnManger : MonoBehaviour
{
    [SerializeField]
    private Transform _spawnPosition;
    [SerializeField]
    private Transform _enemyParent;

    [SerializeField]
    private SpawnTime[] _stepBySpawnTime;

    private int _curStep = 0;
    private float _spawnTime = 3f;
    private float _spawnTimer = 0f;
    
    private void Start()
    {
        GetSpawnTime();
    }

    private void Update()
    {
        _spawnTimer += Time.deltaTime;
        if(_spawnTimer >= _spawnTime)
        {
            Spawn();
        }
    }

    private void GetSpawnTime()
    {
        float minTime = _stepBySpawnTime[_curStep].MinTime; 
        float maxTime = _stepBySpawnTime[_curStep].MaxTime;

        float time = Random.Range(minTime, maxTime);

        _spawnTime = time;
    }
    
    private void Spawn()
    {
        _spawnTimer = 0f;
        GetSpawnTime();

        Debug.Log("Spawn");
    }
}
