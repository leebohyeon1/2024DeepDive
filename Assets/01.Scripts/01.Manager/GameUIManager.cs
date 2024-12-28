using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [SerializeField]
    public Slider[] GameSliders;

    [SerializeField]
    private GameObject _gameOverUI;
    [SerializeField]
    private TMP_Text[] _gameOverTexts;
    [SerializeField]
    private Button[] _gameOverBtns;

    [SerializeField]
    private GameObject _optionUI;
   
    void Start()
    {

        _gameOverBtns[0].onClick.AddListener(() => { RestartBtn(); });
        _gameOverBtns[1].onClick.AddListener(() => { TitleBtn(); });

        _gameOverUI.SetActive(false);
        _optionUI.SetActive(false);
    }

    void Update()
    {
        if(!_optionUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            OpenOption();
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
        SceneManager.LoadScene(1);
    }

    private void TitleBtn()
    {
        SceneManager.LoadScene(0);
    }


    private void OpenOption()
    {
        Time.timeScale = 0.0f;
        _optionUI.SetActive(true);
    }
}
