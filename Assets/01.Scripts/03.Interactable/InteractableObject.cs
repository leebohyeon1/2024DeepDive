using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InteractTime
{
    public float MinTime;
    public float MaxTime;

    public float WaitTime;

    public int AngryRate;
}

public class InteractableObject : MonoBehaviour
{
    public int InteractType = 0;

    [SerializeField]
    protected InteractTime[] _interactTimes;
    protected float _interactTime = 0f; // 이벤트 등장 시간
    protected float _interactTimer = 0f;

    protected int _curStep = 0; // 현재 스텝

    [SerializeField]
    protected float _increaseStepTime = 0f; // 이벤트 단계 증가 시간
    protected float _increaseStepTimer = 0f;

    public bool CanInteract { get; private set; } = false; // 상호작용 할 수 있는지
    protected bool _isMaxStep = false;

    private float _waitTimer = 0f;  // WaitTime 타이머
    private bool _waiting = false;  // WaitTime 대기 상태 플래그


    protected virtual void Start()
    {
        SetEventTime();
    }

    protected virtual void Update()
    {
        _interactTimer += Time.deltaTime;
        if(!CanInteract && _interactTimer >= _interactTime)
        {
            _interactTimer = 0f;
            SetEventTime();

            Debug.Log(name + " 이벤트 발동");

            CanInteract = true;
            _waiting = true;
            _waitTimer = 0f;  // 대기 타이머 초기화
        }

        // WaitTime 카운트
        if (CanInteract && _waiting)
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= _interactTimes[_curStep].WaitTime)
            {
                GameManager.Instance.Angry(_interactTimes[_curStep].AngryRate);
                _waiting = false;
            }
        }

        if (_isMaxStep)
        {
            return;
        }

        _increaseStepTimer += Time.deltaTime;
        if (_increaseStepTimer >= _increaseStepTime)
        {
            NextStep();
        }

    }

    public virtual void Interact()
    {

    }

    protected virtual void SetEventTime()
    {
        _interactTime = Random.Range(_interactTimes[_curStep].MinTime, _interactTimes[_curStep].MaxTime);
    }

    private void NextStep()
    {
        if (_curStep >= _interactTimes.Length - 1)
        {
            _isMaxStep = true;
            return;
        }

        _increaseStepTimer = 0f;
        _curStep++;

    }
}
