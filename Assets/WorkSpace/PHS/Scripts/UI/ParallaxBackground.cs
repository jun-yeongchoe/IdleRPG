using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 텍스터 설정: 기존 룰 취소하고 다시 일반적인 2d sprite로 넣기
/// </summary>

public class ParallaxBackground : MonoBehaviour
{
    [Header("스크롤 속도")]
    public float scrollSpeed = 2f;

    [Header("챕터별 배경 텍스처(순서대로)")]
    public Sprite[] chapterSprites;

    private SpriteRenderer sprite1;
    private SpriteRenderer sprite2;

    private float width;

    private int lastStageNum = -1;

    void Start()
    {
        sprite1 = GetComponent<SpriteRenderer>();

        GameObject clone = new GameObject("BgClone");
        clone.transform.SetParent(transform);
        clone.transform.localPosition = Vector3.zero;

        sprite2 = clone.AddComponent<SpriteRenderer>();
        sprite2.sortingLayerID = sprite1.sortingLayerID;
        sprite2.sortingOrder = sprite1.sortingOrder;

        UpdateBackgroundTexture();
    }

    void Update()
    {
        if (DataManager.Instance != null && DataManager.Instance.currentStageNum != lastStageNum)
        {
            UpdateBackgroundTexture();
        }

        sprite1.transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);
        sprite2.transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        if (sprite1.transform.localPosition.x <= -width)
        {
            sprite1.transform.Translate(Vector3.right * width * 2f);
        }

        if (sprite2.transform.localPosition.x <= -width)
        {
            sprite2.transform.Translate(Vector3.right * width * 2f);
        }
    }

    public void UpdateBackgroundTexture()
    {
        if (DataManager.Instance == null || chapterSprites.Length == 0) return;

        //현재 스테이지 번호 가져오기 (예: 1~10은 챕터1, 11~20은 챕터2)
        int currentStage = DataManager.Instance.currentStageNum;

        lastStageNum = currentStage;

        int index = ((currentStage - 1) / 10) % chapterSprites.Length;

        Sprite currentSprite = chapterSprites[index];
        sprite1.sprite = currentSprite;
        sprite2.sprite = currentSprite;

        width = currentSprite.bounds.size.x;

        //배열 인덱스 계산 (0부터 시작하므로 -1 후 10으로 나눔)
        //1~10 -> index 0 (챕터1)
        //11~20 -> index 1 (챕터2)
        //21~30 -> index 2 (챕터3)
        sprite1.transform.localPosition = Vector3.zero;
        sprite2.transform.localPosition = new Vector3(width, 0, 0);
    }
}
