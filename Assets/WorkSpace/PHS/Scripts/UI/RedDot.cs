using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedDot : MonoBehaviour
{
    private GameObject redDot;
    // Start is called before the first frame update
    void Start()
    {
        redDot = gameObject;

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.questUpdateEvent += CheckRedDot;
        }

        CheckRedDot();
    }

    private void OnEnable()
    {
        CheckRedDot();
    }

    public void CheckRedDot()
    {
        if (QuestManager.Instance == null) return;

        bool isReady=QuestManager.Instance.CheckReward();
        redDot.SetActive(isReady);
    }

    private void OnDestroy()
    {
        if (QuestManager.Instance != null) 
        { 
            QuestManager.Instance.questUpdateEvent -= CheckRedDot;
        }   
    }
}
