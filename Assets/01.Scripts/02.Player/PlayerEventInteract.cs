using UnityEngine;
using UnityEngine.UI;

public class PlayerEventInteract : MonoBehaviour
{
    public bool IsInteract { get; private set; }

    private int _interactType = 0;

    [Header("�丮")]
    [SerializeField] private float _sliderSpeed = 2f;
    [SerializeField] private float _cookRange = 0.1f;

    private Slider _cookSlider;
    private RectTransform _targetZone;

    private float _sliderValue = 0.5f;
    private float _direction = 1f;
    private float _targetMin = 0f;
    private float _targetMax = 0f;

    [Space(20f), Header("������")]
    [SerializeField] private float _decreaseRate = 10f;
    [SerializeField] private float _increasePerPress = 10f;

    private Slider _washSlider;
    private float _gauge = 10f;
    private const float _maxGauge = 100f;

    [Space(20f), Header("�Ʊ� �޷���")]
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

    // �̴ϰ��� UI Ȱ��ȭ
    private void ActivateSliderUI(int type)
    {
        switch (type)
        {
            case 0: StartCook(); break;
            case 1: _washSlider.gameObject.SetActive(true); break;
            case 2: _babySlider.gameObject.SetActive(true); break;
        }
    }

    // �丮 �̴ϰ��� ����
    private void StartCook()
    {
        _cookSlider.gameObject.SetActive(true);
        _sliderValue = 0.5f;
        _cookSlider.value = _sliderValue;

        // ���� ���� ����
        _targetMin = Random.Range(0.1f, 1f - _cookRange);
        _targetMax = _targetMin + _cookRange;

        UpdateTargetZoneUI();
    }

    // �丮 �̴ϰ��� ����
    private void Cook()
    {
        _sliderValue += _direction * _sliderSpeed * Time.deltaTime;
        _sliderValue = Mathf.Clamp01(_sliderValue);
        _cookSlider.value = _sliderValue;

        if (_sliderValue >= 1f || _sliderValue <= 0f)
        {
            _direction *= -1f;
        }

        // E Ű�� ���� �ȿ��� Ȯ��
        if (Input.GetKeyDown(KeyCode.E) && _sliderValue >= _targetMin && _sliderValue <= _targetMax)
        {
            ClearGame(EVENT_TYPE.COOK, _cookSlider);
        }
    }

    // ������ �̴ϰ��� ����
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

    // �Ʊ� �޷��� �̴ϰ��� ����
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

    // �̴ϰ��� Ŭ����
    private void ClearGame(EVENT_TYPE eventType, Slider slider)
    {
        slider.gameObject.SetActive(false);
        IsInteract = false;
        AudioManager.Instance.PlaySFX("Action_Success");
        GameManager.Instance.IncreaseInteractCount();
        EventManager.Instance.PostNotification(eventType, this);
        EventManager.Instance.PostNotification(EVENT_TYPE.STOP_INTERACT, this);
    }

    // ���� ���� ó��
    public void TriggerGameOver()
    {
        IsInteract = false;
        ResetValues();
        DeactivateAllUI();
    }

    // ��� UI ��Ȱ��ȭ
    private void DeactivateAllUI()
    {
        _cookSlider.gameObject.SetActive(false);
        _washSlider.gameObject.SetActive(false);
        _babySlider.gameObject.SetActive(false);
    }

    // �ʱ�ȭ
    private void ResetValues()
    {
        _gauge = 10f;
        _holdTime = 0f;
        _sliderValue = 0.5f;
    }

    // ���� UI ������Ʈ
    private void UpdateTargetZoneUI()
    {
        float sliderWidth = _cookSlider.GetComponent<RectTransform>().rect.width;
        _targetZone.gameObject.SetActive(true);

        float centerX = (_targetMin + _targetMax) / 2 * sliderWidth - sliderWidth / 2;
        _targetZone.anchoredPosition = new Vector2(centerX, _targetZone.anchoredPosition.y);
        _targetZone.sizeDelta = new Vector2((_targetMax - _targetMin) * sliderWidth, _targetZone.sizeDelta.y);
    }

    // �ܺο��� UI �Ҵ�
    public void SetUI(Slider cookSlider, RectTransform targetZone, Slider washSlider, Slider babySlider)
    {
        _cookSlider = cookSlider;
        _targetZone = targetZone;
        _washSlider = washSlider;
        _babySlider = babySlider;
    }
}
