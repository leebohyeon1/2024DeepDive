using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>, IListener
{
    [SerializeField]
    private int _princessAngryRate;

    [SerializeField]
    private Transform[] _eventObjects;

    private bool _isGameOver = false;

    [SerializeField]
    private int _houseHp = 0;
    private int _curHouseHp = 0;

    private int _killCount;
    private int _InteractCount;
    private float _gametime;

    protected override void Start()
    {
        InitialGame();
    }

    protected override void Update()
    {
        if (_isGameOver)
        {
            return;
        }

        _gametime += Time.deltaTime;
    }

    private void InitialGame()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.GAME_OVER, this);

        _curHouseHp = _houseHp;
    }

    public void OnEvent(EVENT_TYPE Event_type, Component Sender, object Param = null)
    {
        switch (Event_type)
        {
            case EVENT_TYPE.GAME_OVER:
                GameOver();
                break;
        }

    }

    private void GameOver()
    {
        if (_isGameOver)
        {
            return;
        }

        _isGameOver = true;
        Debug.Log("GameOver");
    }


    public void Angry(int rate)
    {
        _princessAngryRate += rate;

        if (_princessAngryRate >= 100)
        {
            GameOver();
        }
    }

    public Transform[] GetEventObjs()
    {
        return _eventObjects;
    }

    public void TakeHouseDamage(int damage)
    {
        _curHouseHp -= damage;

        if (_curHouseHp <= 0)
        {
            GameOver();
        }
    }

    public void IncreaseInteractCount()
    {
        _InteractCount++;
    }

    public void IncreaseKillCount()
    {
        _killCount++;
    }
}

