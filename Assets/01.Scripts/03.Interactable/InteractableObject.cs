using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InteractTime
{
    public float MinTime;
    public float MaxTime;
    public float WaitTime;
    public int AngryRate;
}

public class InteractableObject : MonoBehaviour, IListener
{
    private Animator _animator;

    public int InteractType = 0;

    [SerializeField] private InteractTime[] _interactTimes;
    [SerializeField] private float _increaseStepTime = 0f;  // 단계 증가 시간

    public Slider Slider;

    private float _interactTime = 0f;  // 이벤트 등장 시간
    private float _interactTimer = 0f;
    private float _increaseStepTimer = 0f;
    private float _waitTimer = 0f;

    private int _curStep = 0;
    private bool _isMaxStep = false;
    private bool _waiting = false;
    private bool _canInteract = false;
    private bool _isGameOver = false;

    public bool CanInteract => _canInteract && !_isGameOver;  // 게임 오버 시 false 반환

    protected virtual void Start()
    {
        SetEventTime();
        Slider.gameObject.SetActive(false);
        EventManager.Instance.AddListener(EVENT_TYPE.GAME_OVER, this);

        _animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if (_isGameOver) return;  // 게임 오버 상태에서 Update 중단

        if(!_canInteract && !_waiting)
        {
            _interactTimer += Time.deltaTime;

            if(_interactTimer >= _interactTime)
            {
                TriggerEvent();
            }
        }

        if (_canInteract && _waiting)
        {
            HandleWaitTime();
        }

        if (!_isMaxStep)
        {
            _increaseStepTimer += Time.deltaTime;
            if (_increaseStepTimer >= _increaseStepTime)
            {
                NextStep();
            }
        }
    }

    // 이벤트 발동 처리
    private void TriggerEvent()
    {
        _interactTimer = 0f;
        SetEventTime();
        _canInteract = true;
        _waiting = true;
        _waitTimer = 0f;
        _animator.SetBool("Event",true);

        GameManager.Instance.SetWait(InteractType, true);
    }

    // WaitTime 처리
    private void HandleWaitTime()
    {
        _waitTimer += Time.deltaTime;
        if (_waitTimer >= _interactTimes[_curStep].WaitTime)
        {
            Failed();
        }
    }

    // 이벤트 단계 증가
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

    // 이벤트 클리어 (성공 시 호출)
    protected virtual void Clear()
    {
        _canInteract = false;
        _waiting = false;
        Slider.gameObject.SetActive(false);

        _animator.SetBool("Event", false);
        GameManager.Instance.SetWait(InteractType, false);
    }

    // 이벤트 실패 처리 (시간 초과)
    protected virtual void Failed()
    {
        GameManager.Instance.IncreaseAnger(_interactTimes[_curStep].AngryRate);
        _waiting = false;
        _canInteract = false;

        _animator.SetBool("Event", false);
        GameManager.Instance.SetWait(InteractType, false);
        EventManager.Instance.PostNotification(EVENT_TYPE.STOP_INTERACT, this);
    }

    // 랜덤으로 이벤트 타이밍 설정
    protected virtual void SetEventTime()
    {
        _interactTime = Random.Range(_interactTimes[_curStep].MinTime, _interactTimes[_curStep].MaxTime);
    }

    // 게임 오버 이벤트 리스너
    public virtual void OnEvent(EVENT_TYPE Event_type, Component Sender, object Param = null)
    {
        if (Event_type == EVENT_TYPE.GAME_OVER)
        {
            _isGameOver = true;
        }
        else
        {
            Clear();
        }
    }
}
