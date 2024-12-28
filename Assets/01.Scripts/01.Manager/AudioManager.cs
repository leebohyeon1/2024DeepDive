using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AudioManager : Singleton<AudioManager>
{
    [Header("BGM Settings")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private List<AudioClip> _bgmClips;

    [Header("SFX Settings")]
    [SerializeField] private int _sfxSourceCount = 5; // SFX Ǯ���� ���� ����� �ҽ� ����
    [SerializeField] private List<AudioClip> _sfxClips;

    private List<AudioSource> _sfxSources = new List<AudioSource>();

    private Dictionary<string, AudioClip> _bgmDictionary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> _sfxDictionary = new Dictionary<string, AudioClip>();

    private bool _isBgmMuted = false;
    private bool _isSfxMuted = false;
    private int currentSfxIndex = 0; // ���� ��� ���� SFX �ҽ� �ε���

    protected override void Awake()
    {
        base.Awake();
    }
    
    protected override void Start()
    {
        InitializeAudioSources();
        InitializeAudioClips();
        LoadAudioSettings(); // PlayerPrefs���� ���� �ҷ�����
    }

    private void InitializeAudioSources()
    {
        if (_bgmSource == null)
        {
            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.loop = true;
            _bgmSource.priority = 64;
        }

        for (int i = 0; i < _sfxSourceCount; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.priority = 128;
            source.dopplerLevel = 0f;
            _sfxSources.Add(source);
        }
    }

    private void InitializeAudioClips()
    {
        foreach (var clip in _bgmClips)
        {
            if (!_bgmDictionary.ContainsKey(clip.name))
            {
                _bgmDictionary.Add(clip.name, clip);
            }
        }

        foreach (var clip in _sfxClips)
        {
            if (!_sfxDictionary.ContainsKey(clip.name))
            {
                _sfxDictionary.Add(clip.name, clip);
            }
        }
    }

    #region BGM Methods

    public bool CheckCurBGM(string clipName)
    {
        return _bgmSource.clip == _bgmDictionary[clipName] ? true : false;
    }

    public void PlayBGM(string clipName, bool loop = true)
    {
        if (_bgmDictionary.ContainsKey(clipName))
        {
            _bgmSource.clip = _bgmDictionary[clipName];
            _bgmSource.loop = loop;
            _bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"BGM Ŭ�� '{clipName}'��(��) ã�� �� �����ϴ�.");
        }
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
    }

    public void SetBgmVolume(float volume)
    {
        _bgmSource.volume = Mathf.Clamp01(volume);
    }

    public void MuteBGM(bool mute)
    {
        _isBgmMuted = mute;
        _bgmSource.mute = mute;
    }

    public bool IsBgmMuted()
    {
        return _isBgmMuted;
    }

    #endregion

    #region SFX Methods
    public void PlaySFX(string clipName, bool isloop = false)
    {
        if (_sfxDictionary.ContainsKey(clipName))
        {
            AudioSource source = _sfxSources[currentSfxIndex];

            if (isloop)
            {
                source.clip = _sfxDictionary[clipName];
                source.loop = true;
                source.Play();
            }
            else
            {
                source.PlayOneShot(_sfxDictionary[clipName]);
            }

            currentSfxIndex = (currentSfxIndex + 1) % _sfxSources.Count; // ���� �ε����� �̵�
        }
        else
        {
            Debug.LogWarning($"SFX Ŭ�� '{clipName}'��(��) ã�� �� �����ϴ�.");
        }
    }

    public void StopSFX(string clipName)
    {
        foreach (var source in _sfxSources)
        {
            if (source.isPlaying && source.clip != null && source.clip.name == clipName)
            {
                source.Stop();
                source.loop = false; // ���� ����
            }
        }
    }

    public void SetSfxVolume(float volume)
    {
        foreach (var source in _sfxSources)
        {
            source.volume = Mathf.Clamp01(volume);
        }
    }

    public void MuteSFX(bool mute)
    {
        _isSfxMuted = mute;
        foreach (var source in _sfxSources)
        {
            source.mute = mute;
        }
    }

    public bool IsSfxMuted()
    {
        return _isSfxMuted;
    }

    #endregion

    #region BgmFadeInOut
    public void FadeInBGM(string clipName, float duration)
    {
        StartCoroutine(FadeInCoroutine(clipName, duration));
    }

    private IEnumerator FadeInCoroutine(string clipName, float duration)
    {
        PlayBGM(clipName);
        _bgmSource.volume = 0f;
        float timer = 0f;
        while (timer < duration)
        {
            _bgmSource.volume = Mathf.Lerp(0f, 1f, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        _bgmSource.volume = 1f;
    }

    public void FadeOutBGM(float duration)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = _bgmSource.volume;
        float timer = 0f;
        while (timer < duration)
        {
            _bgmSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        _bgmSource.volume = 0f;
        StopBGM();
    }
    #endregion

    #region PlayerPrefs Methods

    public void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("BgmVolume", _bgmSource.volume);
        PlayerPrefs.SetFloat("SfxVolume", _sfxSources[0].volume); // SFX�� ������ ������ �����Ѵٰ� ����
        PlayerPrefs.SetInt("BgmMuted", _isBgmMuted ? 1 : 0);
        PlayerPrefs.SetInt("SfxMuted", _isSfxMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadAudioSettings()
    {
        if (PlayerPrefs.HasKey("BgmVolume"))
        {
            SetBgmVolume(PlayerPrefs.GetFloat("BgmVolume"));
        }

        if (PlayerPrefs.HasKey("SfxVolume"))
        {
            SetSfxVolume(PlayerPrefs.GetFloat("SfxVolume"));
        }

        if (PlayerPrefs.HasKey("BgmMuted"))
        {
            MuteBGM(PlayerPrefs.GetInt("BgmMuted") == 1);
        }

        if (PlayerPrefs.HasKey("SfxMuted"))
        {
            MuteSFX(PlayerPrefs.GetInt("SfxMuted") == 1);
        }
    }

    #endregion
}