using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEventInteract : MonoBehaviour
{
    public bool IsInteract { get; private set; }

    private int _interactType = 0;

    [Header("요리")]
    [SerializeField]
    private float _sliderSpeed = 2f;
    [SerializeField]
    private float _cookRange = 0.1f;

    private Slider _cookSlider; // 슬라이더 UI
    private RectTransform _targetZone; // 랜덤 범위 UI

    private float _sliderValue = 0f;
    private float _direction = 1f;
    private float _targetMin = 0f;
    private float _targetMax = 0f;

    // 설거지
    [Space(20f),Header("설거지")]
    [SerializeField]
    private float _decreaseRate = 10f; // 초당 감소량
    [SerializeField]
    private float _increasePerPress = 10f; // 연타 시 증가량

    private Slider _washSlider; // 슬라이더 UI

    private float _gauge = 10f;
    private float _maxGauge = 100f;



    [Space(20f), Header("아기 달래기")]
    [SerializeField]
    private float _targetHoldTime = 3f; // 3초
    private float _holdTime = 0f;

    private Slider _babySlider; // 슬라이더 UI

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
        if ( _interactType == 0)
        {
            StartCook();  // 미니게임 0 시작
        }
        else if(_interactType == 1)
        {
            _washSlider.gameObject.SetActive(true);
        }
        else
        {
            _babySlider.gameObject.SetActive(true);
        }
    }

    private void PerformMiniGame(int type)
    {
        switch (type)
        {
            case 0:
                Cook();
                break;

            case 1:
                WashDishes();
                break;

            case 2:
                BabySeating();
                break;
        }
    }

    private void Cook()
    {

        // 슬라이더 이동
        _sliderValue += _direction * _sliderSpeed * Time.deltaTime;
        _cookSlider.value =Mathf.Clamp(_sliderValue,0f,1f);

        // 방향 반전 (슬라이더 끝에 닿았을 때)
        if (_sliderValue >= 1f )
        {
            _direction = -1f;
        }
        else if(_sliderValue <= 0f)
        {
            _direction = 1f;
        }

        // E 키로 범위 안에서 확인
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_sliderValue >= _targetMin && _sliderValue <= _targetMax)
            {
                _cookSlider.gameObject.SetActive(false);
               TriggerGameClear();
                EventManager.Instance.PostNotification(EVENT_TYPE.COOK, this);
            }
        }
    }
    private void StartCook()
    {
        _cookSlider.gameObject.SetActive(true);
        _sliderValue = 0.5f;  // 중앙에서 시작
        _cookSlider.value = _sliderValue;

        // 랜덤 범위 설정
        _targetMin = Random.Range(0.1f, 1f - _cookRange);
        _targetMax = _targetMin + _cookRange;  // 범위 0.1 크기

        // 시각적으로 범위 표시 (RectTransform 조절)
        SetTargetZoneUI();
    }

    // 범위 표시
    private void SetTargetZoneUI()
    {
        float sliderWidth = _cookSlider.GetComponent<RectTransform>().rect.width;
        _targetZone.gameObject.SetActive(true);

        float minX = _targetMin * sliderWidth - sliderWidth / 2;
        float maxX = _targetMax * sliderWidth - sliderWidth / 2;
        float centerX = (minX + maxX) / 2;

        _targetZone.anchoredPosition = new Vector2(centerX, _targetZone.anchoredPosition.y);
        _targetZone.sizeDelta = new Vector2((maxX - minX), _targetZone.sizeDelta.y);
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

                _washSlider.value = _gauge / 100;
                _washSlider.gameObject.SetActive(false);
                return;
            }
        }

        // 게이지 감소
        _gauge -= _decreaseRate * Time.deltaTime;
        if (_gauge < 0)
        {
            _gauge = 0;
        }

        _washSlider.value = _gauge / 100;
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
                _babySlider.gameObject.SetActive(false);
                EventManager.Instance.PostNotification(EVENT_TYPE.BABY_SEAT, this);
            }
        }
        else
        {
            _holdTime = 0;
        }

        _babySlider.value = _holdTime / _targetHoldTime;
    }

    public void TriggerGameOver()
    {
        IsInteract = false;

        _gauge = 10;
        _holdTime = 0;

        _cookSlider.gameObject.SetActive(false);
        _washSlider.gameObject.SetActive(false);
        _babySlider.gameObject.SetActive(false);
    }

    private void TriggerGameClear()
    {
        IsInteract = false;
        GameManager.Instance.IncreaseInteractCount();
    }

    public void SetUI(Slider cookSlider, RectTransform targetZone, Slider washSlider, Slider babySlider)
    {
        _cookSlider = cookSlider;
        _targetZone = targetZone;
        _washSlider = washSlider;
        _babySlider = babySlider;
    }
}
