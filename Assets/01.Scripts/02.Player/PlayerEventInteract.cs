using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventInteract : MonoBehaviour
{
    public bool IsInteract { get; private set; }

    private int _interactType = 0;

    // ������
    [Header("������")]
    private float _gauge = 10f;
    private float _maxGauge = 100f;
    [SerializeField]
    private float _decreaseRate = 10f; // �ʴ� ���ҷ�
    [SerializeField]
    private float _increasePerPress = 10f; // ��Ÿ �� ������


    [Header("�Ʊ� �޷���")]
    private float _holdTime = 0f;
    private float _targetHoldTime = 3f; // 3��

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
        // E Ű ��Ÿ �� ������ ����
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

        // ������ ����
        _gauge -= _decreaseRate * Time.deltaTime;
        if (_gauge < 0)
        {
            _gauge = 0;
        }
    }

    private void BabySeating()
    {
        // E Ű�� ������ ���� ��
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
