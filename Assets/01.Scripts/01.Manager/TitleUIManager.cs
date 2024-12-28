using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;  // DOTween 사용

public class TitleUIManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform _titleTrans;

    [SerializeField]
    private Button _startBtn;
    [SerializeField]
    private Button _optionBtn;
    [SerializeField]
    private Button _exitBtn;

    [SerializeField]
    private GameObject _option;

    [SerializeField] private float floatAmount = 30f;  // 떠다니는 높이 (Y 이동량)
    [SerializeField] private float floatDuration = 2f; // 떠다니는 속도 (왕복 시간)

    private void Start()
    {
        _startBtn.onClick.AddListener(() => { StartGame(); });
        _optionBtn.onClick.AddListener(() => { OpenOption(); });
        _exitBtn.onClick.AddListener(() => { Exit(); });

        AudioManager.Instance.PlayBGM("Title");

        // 타이틀 둥실둥실 떠다니는 애니메이션 시작
        StartFloatingAnimation();

        Time.timeScale = 1.0f;
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
        AudioManager.Instance.PlaySFX("ClickSound");
        SceneManager.LoadScene(1);
    }

    private void OpenOption()
    {
        AudioManager.Instance.PlaySFX("ClickSound");
        _option.SetActive(true);
    }

    private void Exit()
    {
        AudioManager.Instance.PlaySFX("ClickSound");
        Application.Quit();
    }

    // 타이틀 텍스트 둥실둥실 애니메이션
    private void StartFloatingAnimation()
    {
        // 현재 Y 위치 저장
        float startY = _titleTrans.localPosition.y;

        // 위로 이동
        _titleTrans.DOLocalMoveY(startY + floatAmount, floatDuration)
            .SetEase(Ease.InOutSine)  // 부드럽게 시작하고 끝남
            .SetLoops(-1, LoopType.Yoyo);  // 무한 반복 (위아래 왕복)
    }
}
