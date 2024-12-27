using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEventInteract : MonoBehaviour
{
    public bool IsInteract { get; private set; }

    private int _interactType = 0;

    [Header("�丮")]
    [SerializeField]
    private float _sliderSpeed = 2f;
    [SerializeField]
    private float _cookRange = 0.1f;

    private Slider _cookSlider; // �����̴� UI
    private RectTransform _targetZone; // ���� ���� UI

    private float _sliderValue = 0f;
    private float _direction = 1f;
    private float _targetMin = 0f;
    private float _targetMax = 0f;

    // ������
    [Space(20f),Header("������")]
    [SerializeField]
    private float _decreaseRate = 10f; // �ʴ� ���ҷ�
    [SerializeField]
    private float _increasePerPress = 10f; // ��Ÿ �� ������

    private Slider _washSlider; // �����̴� UI

    private float _gauge = 10f;
    private float _maxGauge = 100f;



    [Space(20f), Header("�Ʊ� �޷���")]
    [SerializeField]
    private float _targetHoldTime = 3f; // 3��
    private float _holdTime = 0f;

    private Slider _babySlider; // �����̴� UI

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
            StartCook();  // �̴ϰ��� 0 ����
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

        // �����̴� �̵�
        _sliderValue += _direction * _sliderSpeed * Time.deltaTime;
        _cookSlider.value =Mathf.Clamp(_sliderValue,0f,1f);

        // ���� ���� (�����̴� ���� ����� ��)
        if (_sliderValue >= 1f )
        {
            _direction = -1f;
        }
        else if(_sliderValue <= 0f)
        {
            _direction = 1f;
        }

        // E Ű�� ���� �ȿ��� Ȯ��
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
        _sliderValue = 0.5f;  // �߾ӿ��� ����
        _cookSlider.value = _sliderValue;

        // ���� ���� ����
        _targetMin = Random.Range(0.1f, 1f - _cookRange);
        _targetMax = _targetMin + _cookRange;  // ���� 0.1 ũ��

        // �ð������� ���� ǥ�� (RectTransform ����)
        SetTargetZoneUI();
    }

    // ���� ǥ��
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
        // E Ű ��Ÿ �� ������ ����
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

        // ������ ����
        _gauge -= _decreaseRate * Time.deltaTime;
        if (_gauge < 0)
        {
            _gauge = 0;
        }

        _washSlider.value = _gauge / 100;
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
