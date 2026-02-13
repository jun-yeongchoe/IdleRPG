using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMergeHandler : MonoBehaviour
{
    public static ItemMergeHandler instance{ get; private set; }

    [Header("Merge Setting")]
    public int requiredCount;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // MergeItems()
    public void MergeItems(int ID, Dictionary<int, int> targetDict)
    {
        if(ID == 0) return;

        int currentId = ID;

        int curreentLv = 0; // DataManager에서 아이템 레벨을 가져와야함

        // int requiredCount = item.GetRequiredComposeCount(curreentLv);
        // 여기에는 Item SO 파일들을 리스트에 저장하고 그중에 ID로 검색해서 일치하는걸 가져오는걸로 바꿔야할거같다.

        int ownedCount = targetDict[currentId];

        if(ownedCount < requiredCount)
        {
            Debug.Log("아이템 합성 실패: 재료 부족");
            return;
        }
        
        targetDict[currentId] -= requiredCount;
        //DataManager.Instance.

        



    }

}
