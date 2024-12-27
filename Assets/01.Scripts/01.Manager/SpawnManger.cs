using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnTime
{
    public float MinTime;
    public float MaxTime;

    public int MinEnemyCount;
    public int MaxEnemyCount;

    public GameObject[] SpawnEnemies;
}

public class SpawnManger : MonoBehaviour
{
    [SerializeField]
    private Transform _spawnPosition; // 적 스폰 위치
    [SerializeField]
    private Transform _enemyParent; 

    [SerializeField]
    private SpawnTime[] _stepBySpawnTime; // 스텝별 적 스폰 시간

    private int _curStep = 0; // 현재 스텝

    private float _spawnTime = 3f; // 스폰 시간
    private float _spawnTimer = 0f;

    [SerializeField]
    private float _increaseStepTime; // 스텝 증가 시간
    private float _stepTimer = 0f;
    private bool _isMaxStep = false;

    private void Start()
    {
        SetSpawnTime();
    }

    private void Update()
    {
        _spawnTimer += Time.deltaTime;
        if(_spawnTimer >= _spawnTime)
        {
           StartCoroutine(Spawn());
        }

        if(_isMaxStep)
        {
            return;
        }

        _stepTimer += Time.deltaTime;
        if(_stepTimer >= _increaseStepTime)
        {
            NextStep();
        }
    }

    private void NextStep()
    {
        if (_curStep >= _stepBySpawnTime.Length - 1)
        {
            _isMaxStep = true;
            return;
        }

        _stepTimer = 0f;
        _curStep++;

    }

    private void SetSpawnTime()
    {
        float minTime = _stepBySpawnTime[_curStep].MinTime; 
        float maxTime = _stepBySpawnTime[_curStep].MaxTime;

        float time = Random.Range(minTime, maxTime);

        _spawnTime = time;
    }
    
    private IEnumerator Spawn()
    {
        _spawnTimer = 0f;
        SetSpawnTime();

        for(int i = 0; i < Random.Range(_stepBySpawnTime[_curStep].MinEnemyCount, _stepBySpawnTime[_curStep].MaxEnemyCount+1); i++)
        {
            yield return new WaitForSeconds(0.3f);
            GameObject spawnEnemy = _stepBySpawnTime[_curStep].SpawnEnemies[Random.Range(0, _stepBySpawnTime[_curStep].SpawnEnemies.Length)];
            Instantiate(spawnEnemy, _spawnPosition.position, Quaternion.identity, _enemyParent);
        }
    }
}
