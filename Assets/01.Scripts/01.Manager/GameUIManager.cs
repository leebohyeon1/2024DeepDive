using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [Header("게임UI")]
    [SerializeField]
    public Slider[] GameSliders;
    [SerializeField] 
    private GameObject interactionImage;  
    [SerializeField] 
    private Transform houseTransform;
    [SerializeField] private Vector2 screenOffset = new Vector2(-100f, 100f);  // 화면 우측 이미지 위치

    [SerializeField] private TMP_Text _playTimeText;
    private Camera _mainCamera;

    [Header("게임오버UI")]
    [SerializeField]
    private GameObject _gameOverUI;
    [SerializeField]
    private TMP_Text[] _gameOverTexts;
    [SerializeField]
    private Button[] _gameOverBtns;

    [Header("옵션UI")]
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
            interactionImage.GetComponent<RectTransform>().position =
                new Vector2(Screen.width + screenOffset.x, Screen.height + screenOffset.y);
        }
    }

    public void HideInteractionIcon()
    {
        interactionImage.SetActive(false);
    }
}
