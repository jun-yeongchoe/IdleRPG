using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]

public class SoundForUI : MonoBehaviour
{
    [Header("버튼 클릭 사운드")]
    public AudioClip clickClip;

    private void Start()
    {
        //자기 자신(버튼)의 클릭 이벤트에 사운드 재생 함수를 끼워넣음
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (SoundManager.Instance != null && clickClip != null)
            {
                SoundManager.Instance.PlaySFX(clickClip);
            }
        });
    }
}
