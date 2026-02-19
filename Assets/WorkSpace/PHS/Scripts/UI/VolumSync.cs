using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumSync : MonoBehaviour
{
    public enum VolumeType { BGM, SFX }
    public VolumeType volumeType;

    private Slider slider;
    private string prefKey;

    void Start()
    {
        slider = GetComponent<Slider>();

        prefKey = (volumeType == VolumeType.BGM) ? "BGM_Volume" : "SFX_Volume";

        float savedVol = PlayerPrefs.GetFloat(prefKey, 0.5f);
        slider.value = savedVol;

        if (SoundManager.Instance != null)
        {
            if(volumeType == VolumeType.BGM) SoundManager.Instance.bgmVolume = savedVol;
            else SoundManager.Instance.sfxVolume=savedVol;

            SoundManager.Instance.ApplyVolume();
        }

        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        PlayerPrefs.SetFloat(prefKey, value);

        if (SoundManager.Instance == null) return;

        if (volumeType == VolumeType.BGM)
        {
            SoundManager.Instance.bgmVolume=value;
            Debug.Log($"BGM 볼륨 변경됨: {value}");
        }
        else
        {
            SoundManager.Instance.bgmVolume = value;
            Debug.Log($"SFX 볼륨 변경됨: {value}");
        }

        SoundManager.Instance.ApplyVolume();
    }
}
