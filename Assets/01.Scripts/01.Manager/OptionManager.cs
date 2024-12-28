using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    [SerializeField]
    private Button _exitOptionBtn;

    [SerializeField]
    private Slider _bgmSlider;
    [SerializeField]
    private Slider _sfxSlider;
    [SerializeField]
    private Button _titleBtn;


    private void Start()
    {
        _exitOptionBtn.onClick.AddListener(() => { ExitOption(); });

        _bgmSlider.onValueChanged.AddListener(delegate { BgmVolumeChange(_bgmSlider.value); });
        _sfxSlider.onValueChanged.AddListener(delegate { SfxVolumeChange(_sfxSlider.value); });

        _titleBtn.onClick.AddListener(() => { SceneManager.LoadScene(0); });
    }

    private void OnEnable()
    {
        ResetOption();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ExitOption();
        }
    }

    private void ExitOption()
    {
        Time.timeScale = 1.0f;
        AudioManager.Instance.PlaySFX("ClickSound");
        gameObject.SetActive(false);

        AudioManager.Instance.SaveAudioSettings();
    }

    private void BgmVolumeChange(float volume)
    {
        AudioManager.Instance.SetBgmVolume(volume);
    }


    private void SfxVolumeChange(float volume)
    {
        AudioManager.Instance.SetSfxVolume(volume);
    }

    public void ResetOption()
    {
        AudioManager.Instance.LoadAudioSettings();

        _bgmSlider.value = PlayerPrefs.GetFloat("BgmVolume");
        _sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume");
    }
}
