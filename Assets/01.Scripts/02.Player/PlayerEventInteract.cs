using UnityEngine;
using UnityEngine.UI;

public class PlayerEventInteract : MonoBehaviour
{
    public bool IsInteract { get; private set; }

    private int _interactType = 0;

    [Header("요리")]
    [SerializeField] private float _sliderSpeed = 2f;
    [SerializeField] private float _cookRange = 0.1f;

    private Slider _cookSlider;
    private RectTransform _targetZone;

    private float _sliderValue = 0.5f;
    private float _direction = 1f;
    private float _targetMin = 0f;
    private float _targetMax = 0f;

    [Space(20f), Header("설거지")]
    [SerializeField] private float _decreaseRate = 10f;
    [SerializeField] private float _increasePerPress = 10f;

    private Slider _washSlider;
    private float _gauge = 10f;
    private const float _maxGauge = 100f;

    [Space(20f), Header("아기 달래기")]
    [SerializeField] private float _targetHoldTime = 3f;
    private float _holdTime = 0f;

    private Slider _babySlider;

    void Update()
    {
        if (IsInteract)
        {
            PerformMiniGame();
        }
    }

    public void SetInteract(bool state)
    {
        IsInteract = state;
    }

    public void SetInteractType(int type)
    {
        _interactType = type;
        ActivateSliderUI(type);
    }

    private void PerformMiniGame()
    {
        switch (_interactType)
        {
            case 0: Cook(); break;
            case 1: WashDishes(); break;
            case 2: BabySeating(); break;
        }
    }

    // 미니게임 UI 활성화
    private void ActivateSliderUI(int type)
    {
        switch (type)
        {
            case 0: StartCook(); break;
            case 1: _washSlider.gameObject.SetActive(true); break;
            case 2: _babySlider.gameObject.SetActive(true); break;
        }
    }

    // 요리 미니게임 시작
    private void StartCook()
    {
        _cookSlider.gameObject.SetActive(true);
        _sliderValue = 0.5f;
        _cookSlider.value = _sliderValue;

        // 랜덤 범위 설정
        _targetMin = Random.Range(0.1f, 1f - _cookRange);
        _targetMax = _targetMin + _cookRange;

        UpdateTargetZoneUI();
    }

    // 요리 미니게임 진행
    private void Cook()
    {
        _sliderValue += _direction * _sliderSpeed * Time.deltaTime;
        _sliderValue = Mathf.Clamp01(_sliderValue);
        _cookSlider.value = _sliderValue;

        if (_sliderValue >= 1f || _sliderValue <= 0f)
        {
            _direction *= -1f;
        }

        // E 키로 범위 안에서 확인
        if (Input.GetKeyDown(KeyCode.E) && _sliderValue >= _targetMin && _sliderValue <= _targetMax)
        {
            ClearGame(EVENT_TYPE.COOK, _cookSlider);
        }
    }

    // 설거지 미니게임 진행
    private void WashDishes()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            _gauge = Mathf.Min(_gauge + _increasePerPress, _maxGauge);
            _washSlider.value = _gauge / _maxGauge;

            if (_gauge >= _maxGauge)
            {
                ClearGame(EVENT_TYPE.WASH_DISHES, _washSlider);
                _gauge = 10f;
                return;
            }
        }

        _gauge = Mathf.Max(_gauge - _decreaseRate * Time.deltaTime, 0f);
        _washSlider.value = _gauge / _maxGauge;
    }

    // 아기 달래기 미니게임 진행
    private void BabySeating()
    {
        if (Input.GetKey(KeyCode.E))
        {
            _holdTime += Time.deltaTime;
            if (_holdTime >= _targetHoldTime)
            {
                ClearGame(EVENT_TYPE.BABY_SEAT, _babySlider);
                _holdTime = 0;
            }
        }
        else
        {
            _holdTime = 0;
        }

        _babySlider.value = _holdTime / _targetHoldTime;
    }

    // 미니게임 클리어
    private void ClearGame(EVENT_TYPE eventType, Slider slider)
    {
        slider.gameObject.SetActive(false);
        IsInteract = false;
        AudioManager.Instance.PlaySFX("Action_Success");
        GameManager.Instance.IncreaseInteractCount();
        EventManager.Instance.PostNotification(eventType, this);
        EventManager.Instance.PostNotification(EVENT_TYPE.STOP_INTERACT, this);
    }

    // 게임 오버 처리
    public void TriggerGameOver()
    {
        IsInteract = false;
        ResetValues();
        DeactivateAllUI();
    }

    // 모든 UI 비활성화
    private void DeactivateAllUI()
    {
        _cookSlider.gameObject.SetActive(false);
        _washSlider.gameObject.SetActive(false);
        _babySlider.gameObject.SetActive(false);
    }

    // 초기화
    private void ResetValues()
    {
        _gauge = 10f;
        _holdTime = 0f;
        _sliderValue = 0.5f;
    }

    // 범위 UI 업데이트
    private void UpdateTargetZoneUI()
    {
        float sliderWidth = _cookSlider.GetComponent<RectTransform>().rect.width;
        _targetZone.gameObject.SetActive(true);

        float centerX = (_targetMin + _targetMax) / 2 * sliderWidth - sliderWidth / 2;
        _targetZone.anchoredPosition = new Vector2(centerX, _targetZone.anchoredPosition.y);
        _targetZone.sizeDelta = new Vector2((_targetMax - _targetMin) * sliderWidth, _targetZone.sizeDelta.y);
    }

    // 외부에서 UI 할당
    public void SetUI(Slider cookSlider, RectTransform targetZone, Slider washSlider, Slider babySlider)
    {
        _cookSlider = cookSlider;
        _targetZone = targetZone;
        _washSlider = washSlider;
        _babySlider = babySlider;
    }
}
