using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Stage Settings")]
    [SerializeField] private GameObject[] chapterSpawner;

    private int currentChapter = -1;

    private void Awake()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player=playerObj.transform;
        }
    }

    // 업데이트에서 스테이지 변경체크
    void Update()
    {
        CheckStage();
    }

    private void CheckStage()
    { 
        if(DataManager.Instance==null) return;

        int dataIndex = (int)DataManager.Instance.BackgroundIndex();

        if (currentChapter == dataIndex) return;

        currentChapter= dataIndex;
        ActivateSpawner(currentChapter);
    }
    private void ActivateSpawner(int index)
    {
        if (index < 0 || index >= chapterSpawner.Length) return;

        for (int i = 0; i < chapterSpawner.Length; i++)
        {
            if (chapterSpawner[i] != null)
            { 
                bool isActive=(i == index);
                chapterSpawner[i].SetActive(isActive);
            }
        }
        Debug.Log($"{index}번 스포너 활성화");
    }

    public Transform GetPlayerTransform()
    {
        return player;
    }
}
