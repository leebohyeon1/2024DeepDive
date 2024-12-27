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

    protected override void Start()
    {
        InitialGame();
    }

    protected override void Update()
    {
        
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
        if(_isGameOver)
        {
            return;
        }

        _isGameOver = true;
        Debug.Log("GameOver");
    }


    private void Angry()
    {

    }

    public Transform[] GetEventObjs()
    {
        return _eventObjects;
    }

    public void TakeHouseDamage(int damage)
    {
        _curHouseHp -= damage;

        if(_curHouseHp <= 0)
        {
            GameOver();
        }
    }

}
