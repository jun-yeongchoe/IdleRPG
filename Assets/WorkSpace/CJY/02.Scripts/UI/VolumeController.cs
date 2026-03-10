using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [Header("Slider References")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    void Start()
    {
        if (SoundManager.Instance != null)
        {
            masterSlider.value = SoundManager.Instance.masterVolume;
            bgmSlider.value = SoundManager.Instance.bgmVolume;
            sfxSlider.value = SoundManager.Instance.sfxVolume;
        }

        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
    }

    private void OnMasterVolumeChanged(float value)
    {
        SoundManager.Instance.masterVolume = value;
        SoundManager.Instance.ApplyVolume();
    }

    private void OnBgmVolumeChanged(float value)
    {
        SoundManager.Instance.bgmVolume = value;
        SoundManager.Instance.ApplyVolume();
    }

    private void OnSfxVolumeChanged(float value)
    {
        SoundManager.Instance.sfxVolume = value;
        SoundManager.Instance.ApplyVolume();
    }
}
