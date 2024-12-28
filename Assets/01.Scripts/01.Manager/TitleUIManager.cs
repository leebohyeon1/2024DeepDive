using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField]
    private Button _startBtn;
    [SerializeField]
    private Button _optionBtn;
    [SerializeField]
    private Button _exitBtn;

    [SerializeField]
    private GameObject _option;

    private void Start()
    {
        _startBtn.onClick.AddListener(() => { StartGame(); });
        _optionBtn.onClick.AddListener(() => { OpenOption(); });
        _exitBtn.onClick.AddListener(() => { Exit(); });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_option.activeSelf)
        {
            OpenOption();
        }
    }

    private void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    private void OpenOption()
    {
        _option.SetActive(true);
    }

    private void Exit()
    {
        Application.Quit();
    }

}
