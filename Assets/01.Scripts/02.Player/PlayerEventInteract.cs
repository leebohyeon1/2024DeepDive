using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventInteract : MonoBehaviour
{
    public bool IsInteract { get; private set; }

    private int _interactType = 0;

    // 설거지
    [Header("설거지")]
    private float _gauge = 10f;
    private float _maxGauge = 100f;
    [SerializeField]
    private float _decreaseRate = 10f; // 초당 감소량
    [SerializeField]
    private float _increasePerPress = 10f; // 연타 시 증가량


    [Header("아기 달래기")]
    private float _holdTime = 0f;
    private float _targetHoldTime = 3f; // 3초

    void Start()
    {
        
    }

    void Update()
    {
        if (IsInteract)
        {
            PerformMiniGame(_interactType);
        }
    }

    public void SetInteract(bool boolean)
    {
        IsInteract = boolean;
    }

    public void SetInteractType(int type)
    {
        _interactType = type;
    }

    private void PerformMiniGame(int type)
    {
        switch (type)
        {
            case 0:

                break;

            case 1:
                WashDishes();
                break;

            case 2:
                BabySeating();
                break;
        }
    }

    private void WashDishes()
    {
        // E 키 연타 시 게이지 증가
        if (Input.GetKeyDown(KeyCode.E))
        {
            _gauge += _increasePerPress;
            if (_gauge > _maxGauge)
            {
                TriggerGameClear();
                EventManager.Instance.PostNotification(EVENT_TYPE.WASH_DISHES, this);
                _gauge = 10;
            }
        }

        // 게이지 감소
        _gauge -= _decreaseRate * Time.deltaTime;
        if (_gauge < 0)
        {
            _gauge = 0;
        }
    }

    private void BabySeating()
    {
        // E 키를 누르고 있을 때
        if (Input.GetKey(KeyCode.E))
        {
            _holdTime += Time.deltaTime;

            if (_holdTime >= _targetHoldTime)
            {
                _holdTime = 0;
                TriggerGameClear();
                EventManager.Instance.PostNotification(EVENT_TYPE.BABY_SEAT, this);
            }
        }
        else
        {
            _holdTime = 0;
        }
    }

    public void TriggerGameOver()
    {
        IsInteract = false;

        _gauge = 10;
        _holdTime = 0;
    }

    private void TriggerGameClear()
    {
        IsInteract = false;
    }
}
