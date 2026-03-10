using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    [Header("BGM")]
    public AudioClip clickClip;

    IEnumerator Start()
    {
        while (SoundManager.Instance == null)
        {
            yield return null;
        }
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(clickClip);
        }
        
    }
}
