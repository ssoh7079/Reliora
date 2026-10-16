using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[DefaultExecutionOrder(-1000)]
public class AudioSettingsUI : MonoBehaviour
{
    [Header("오디오")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("슬라이더")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private const string MasterKey = "Audio_Master";
    private const string BGMKey = "Audio_BGM";
    private const string SFXKey = "Audio_SFX";

    private const string MasterVolume = "MasterVolume";
    private const string BGMVolume = "BGMVolume";
    private const string SFXVolume = "SFXVolume";


    private void Awake()
    {
        float master = PlayerPrefs.GetFloat(MasterKey, 1.0f);
        float bgm = PlayerPrefs.GetFloat(BGMKey, 1.0f);
        float sfx = PlayerPrefs.GetFloat(SFXKey, 1.0f);

        masterSlider.SetValueWithoutNotify(master);
        bgmSlider.SetValueWithoutNotify(bgm);
        sfxSlider.SetValueWithoutNotify(sfx);

        ApplyVolume(MasterVolume, master);
        ApplyVolume(BGMVolume, bgm);
        ApplyVolume(SFXVolume, sfx);

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }
    private void OnDestroy()
    {
        masterSlider.onValueChanged.RemoveListener(SetMasterVolume);
        bgmSlider.onValueChanged.RemoveListener(SetBGMVolume);
        sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
    }

    private void SetMasterVolume(float value)
    {
        ApplyVolume(MasterVolume, value);
        PlayerPrefs.SetFloat(MasterKey, value);
        PlayerPrefs.Save();
    }
    private void SetBGMVolume(float value)
    {
        ApplyVolume(BGMVolume, value);
        PlayerPrefs.SetFloat(BGMKey, value);
        PlayerPrefs.Save();
    }
    private void SetSFXVolume(float value)
    {
        ApplyVolume(SFXVolume, value);
        PlayerPrefs.SetFloat(SFXKey, value);
        PlayerPrefs.Save();
    }
    private void ApplyVolume(string parameter, float value)
    {
        float volume = value <= 0.0001f ? -80.0f : Mathf.Log10(value) * 20.0f;
        audioMixer.SetFloat(parameter, volume);
    }
}
