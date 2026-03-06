using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 텍스터 설정법: 이미지 파일->인스펙터->Wrap Mode 옵션->Repeat로 변경->Apply: 무한 반복됨
/// </summary>

public class ParallaxBackground : MonoBehaviour
{
    [Header("스크롤 속도")]
    public float scrollSpeed = 0.5f;

    [Header("챕터별 배경 텍스처 (순서대로 챕터 1, 2, 3...)")]
    public Texture[] chapterTextures;

    private Material mat;

    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;

        UpdateBackgroundTexture();
    }

    void Update()
    {
        Vector2 offset = mat.mainTextureOffset;
        offset.x += scrollSpeed * Time.deltaTime;
        mat.mainTextureOffset = offset;
    }

    public void UpdateBackgroundTexture()
    {
        if (DataManager.Instance == null || chapterTextures.Length == 0) return;

        //현재 스테이지 번호 가져오기 (예: 1~10은 챕터1, 11~20은 챕터2)
        int currentStage = DataManager.Instance.currentStageNum;

        //배열 인덱스 계산 (0부터 시작하므로 -1 후 10으로 나눔)
        //1~10 -> index 0 (챕터1)
        //11~20 -> index 1 (챕터2)
        //21~30 -> index 2 (챕터3)
        int chapterIndex = ((currentStage - 1) / 10) % chapterTextures.Length;
        mat.mainTexture = chapterTextures[chapterIndex];
    }
}
