using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;  // DOTween ���

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

    [SerializeField] private float floatAmount = 30f;  // ���ٴϴ� ���� (Y �̵���)
    [SerializeField] private float floatDuration = 2f; // ���ٴϴ� �ӵ� (�պ� �ð�)

    private void Start()
    {
        _startBtn.onClick.AddListener(() => { StartGame(); });
        _optionBtn.onClick.AddListener(() => { OpenOption(); });
        _exitBtn.onClick.AddListener(() => { Exit(); });

        AudioManager.Instance.PlayBGM("Title");

        // Ÿ��Ʋ �սǵս� ���ٴϴ� �ִϸ��̼� ����
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

    // Ÿ��Ʋ �ؽ�Ʈ �սǵս� �ִϸ��̼�
    private void StartFloatingAnimation()
    {
        // ���� Y ��ġ ����
        float startY = _titleTrans.localPosition.y;

        // ���� �̵�
        _titleTrans.DOLocalMoveY(startY + floatAmount, floatDuration)
            .SetEase(Ease.InOutSine)  // �ε巴�� �����ϰ� ����
            .SetLoops(-1, LoopType.Yoyo);  // ���� �ݺ� (���Ʒ� �պ�)
    }
}
