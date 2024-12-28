using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameUIManager : MonoBehaviour
{
    [Header("����UI")]
    [SerializeField]
    public Slider[] GameSliders;
    [SerializeField] 
    private GameObject interactionImage;  
    [SerializeField] 
    private Transform houseTransform;
    [SerializeField] private Vector2 screenOffset = new Vector2(-0.1f, 0.1f);  // ������ ���� (ȭ�� ���� ����)
    
    [SerializeField] private TMP_Text _playTimeText;
    private Camera _mainCamera;

    [Header("���ӿ���UI")]
    [SerializeField]
    private GameObject _gameOverUI;
    [SerializeField]
    private TMP_Text[] _gameOverTexts;
    [SerializeField]
    private Button[] _gameOverBtns;

    [Header("�ɼ�UI")]
    [SerializeField]
    private GameObject _optionUI;
   

    void Start()
    {
        _gameOverBtns[0].onClick.AddListener(() => { RestartBtn(); });
        _gameOverBtns[1].onClick.AddListener(() => { TitleBtn(); });

        _gameOverUI.SetActive(false);
        _optionUI.SetActive(false);

        _mainCamera = Camera.main;
    }

    void Update()
    {
        if(GameManager.Instance.GetGameOver())
        {
            return;
        }
        int totalSeconds = GameManager.Instance.GetPlayTime();

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        _playTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (!_optionUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            OpenOption();
        }

        if (interactionImage.activeSelf)
        {
            UpdateImagePosition();
        }
    }

    public void ShowGameOverUI()
    {
        _gameOverUI.SetActive(true);

        int[] score = GameManager.Instance.GetTotalScore();

        int totalSeconds = score[0];
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        _gameOverTexts[0].text = string.Format("{0:00}:{1:00}", minutes, seconds);
        _gameOverTexts[1].text = score[1].ToString();
        _gameOverTexts[2].text = score[2].ToString();
    }

    private void RestartBtn()
    {
        AudioManager.Instance.PlaySFX("ClickSound");
        SceneManager.LoadScene(1);
    }

    private void TitleBtn()
    {
        AudioManager.Instance.PlaySFX("ClickSound");
        SceneManager.LoadScene(0);
    }
 
    private void OpenOption()
    {
        Time.timeScale = 0.0f;
        AudioManager.Instance.PlaySFX("ClickSound");
        _optionUI.SetActive(true);
    }

    public void ShowInteractionIcon()
    {
        interactionImage.SetActive(true);

        // �ʱ� ���� ���� (������ �ʱ�ȭ)
        interactionImage.transform.localScale = Vector3.one;

        // ���� �ִϸ��̼� (��¦ Ŀ���ٰ� ��鸲)
        interactionImage.transform
            .DOScale(Vector3.one * 1.2f, 0.3f)  // 20% Ŀ��
            .SetEase(Ease.OutBack)              // �ε巴�� Ŀ��
            .OnComplete(() =>
            {
                interactionImage.transform.DOShakePosition(0.3f, 10f, 20, 90f, false, true);
            });
    }

    private void UpdateImagePosition()
    {
        Vector3 houseScreenPos = _mainCamera.WorldToScreenPoint(houseTransform.position);

        if (houseScreenPos.z > 0 && houseScreenPos.x >= 0 && houseScreenPos.x <= Screen.width)
        {
            interactionImage.GetComponent<RectTransform>().position =
                (Vector2)houseScreenPos;
        }
        else
        {
            // ȭ�� �ػ󵵿� �°� ������ ��ġ ����
            float xOffset = Screen.width * screenOffset.x;
            float yOffset = Screen.height * screenOffset.y;

            interactionImage.GetComponent<RectTransform>().position =
                new Vector2(Screen.width + xOffset, Screen.height + yOffset);
        }
    }

    public void HideInteractionIcon()
    {
        interactionImage.SetActive(false);
    }
}
