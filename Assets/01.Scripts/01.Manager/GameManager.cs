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
        _isGameOver = true;
    }


    private void Angry()
    {

    }

    public Transform[] GetEventObjs()
    {
        return _eventObjects;
    }

}
