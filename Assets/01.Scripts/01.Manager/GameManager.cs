using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour, IListener
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameUIManager _gameUIManager;

    [SerializeField] private int _princessAngryRate;
    [SerializeField] private Transform[] _eventObjects;

    [SerializeField] private int _houseHp = 0;
    private int _curHouseHp;

    private int _killCount;
    private int _interactCount;
    private float _gameTime;

    private bool _isGameOver = false;
    private bool[] _isWait = new bool[3];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void Start()
    {
        InitializeGame();
    }

    private void Update()
    {
        if (!_isGameOver)
        {
            _gameTime += Time.deltaTime;
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EVENT_TYPE.GAME_OVER, this);
    }

    // ���� �ʱ�ȭ
    private void InitializeGame()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.GAME_OVER, this);
        _curHouseHp = _houseHp;
        Time.timeScale = 1.0f;
    }

    // �̺�Ʈ ������ (���� ���� �̺�Ʈ ó��)
    public void OnEvent(EVENT_TYPE Event_type, Component Sender, object Param = null)
    {
        GameOver();
    }

    // ���� ���� ó��
    private void GameOver()
    {
        if (_isGameOver) return;

        _isGameOver = true;
        Time.timeScale = 0.0f;

        _gameUIManager.ShowGameOverUI();
    }

    // �г� ��ġ ���� ó��
    public void IncreaseAnger(int rate)
    {
        _princessAngryRate += rate;
        float targetValue = _princessAngryRate / 100f;
        _gameUIManager.GameSliders[1].DOValue(targetValue, 0.3f).SetEase(Ease.OutCubic);

        if (_princessAngryRate >= 100)
        {
            _gameUIManager.GameSliders[1].value = 1.0f;
            GameOver();
        }
    }

    // �� ü�� ����
    public void TakeHouseDamage(int damage)
    {
        _curHouseHp = Mathf.Max(_curHouseHp - damage, 0);
        float targetValue = (float)_curHouseHp / _houseHp;
        _gameUIManager.GameSliders[0].DOValue(targetValue, 0.3f).SetEase(Ease.OutCubic);


        if (_curHouseHp == 0)
        {
            _gameUIManager.GameSliders[0].value = 0.0f;
            GameOver();
        }
    }

    // ��ȣ�ۿ� ī��Ʈ ����
    public void IncreaseInteractCount() => _interactCount++;

    // �� óġ ī��Ʈ ����
    public void IncreaseKillCount() => _killCount++;

    // ���� ���� �� �� ���� ��ȯ
    public int[] GetTotalScore()
    {
        return new int[]
        {
            (int)_gameTime,  // �÷��� �ð� (��)
            _killCount,      // �� óġ ��
            _interactCount   // ��ȣ�ۿ� Ƚ��
        };
    }

    // �̺�Ʈ ������Ʈ ��ȯ
    public Transform[] GetEventObjs() => _eventObjects;

    public void SetWait(int index, bool boolean)
    {
        _isWait[index] = boolean;
        CheckWait();
    }

    private void CheckWait()
    {
        bool flag = false;
        foreach(bool isWait in _isWait)
        {
            if(isWait)
            {
                flag = true;
            }
        }

        if(flag)
        {
            _gameUIManager.ShowInteractionIcon();
        }
        else
        {
            _gameUIManager.HideInteractionIcon();
        }
    }
}
