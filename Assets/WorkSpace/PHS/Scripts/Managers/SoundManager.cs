using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Players")]
    [SerializeField] private AudioSource bgmPlayer;
    [SerializeField] private AudioSource sfxPlayer;

    [Range(0, 1)] public float masterVolume = 1.0f;
    [Range(0, 1)] public float bgmVolume = 0.5f;
    [Range(0, 1)] public float sfxVolume = 0.5f;

    private void Awake()
    {
        if (Instance == null) 
        { 
            Instance = this; DontDestroyOnLoad(gameObject); 
            Init(); 
        }
        else
            Destroy(gameObject);
    }

    private void Init()
    {
        //BGM 플레이어 생성
        if (bgmPlayer == null)
        {
            GameObject bgmObj = new ("BgmPlayer");
            bgmObj.transform.parent = transform;
            bgmPlayer = bgmObj.AddComponent<AudioSource>();
        }
        bgmPlayer.loop = true;

        if (sfxPlayer == null)
        {
            GameObject sfxObj = new ("SfxPlayer");
            sfxObj.transform.parent = transform;
            sfxPlayer = sfxObj.AddComponent<AudioSource>();
        }

        ApplyVolume();
    }

    public void ApplyVolume()
    {
        if (bgmPlayer != null) bgmPlayer.volume = masterVolume * bgmVolume;
        if (sfxPlayer != null) sfxPlayer.volume = masterVolume * sfxVolume;
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        bgmPlayer.clip = clip;
        bgmPlayer.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        //타격감
        sfxPlayer.pitch = 1f + Random.Range(-0.1f, 0.1f);

        sfxPlayer.PlayOneShot(clip, masterVolume * sfxVolume);
    }
}
