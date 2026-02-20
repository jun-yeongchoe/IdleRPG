using UnityEngine;

public class PartnerDataBinder : MonoBehaviour
{
    [Header("Binded Data")]
    public PartnerData myData;
    public float currentAtkDamage;
    public float currentAtkSpeed;

    private void Start()
    {
        StartCoroutine(WaitAndBind());
    }

    private System.Collections.IEnumerator WaitAndBind()
    {
        while (PartnerDataLoader.Instance == null || !PartnerDataLoader.Instance.isDataLoaded)
        {
            yield return null;
        }

        BindByIDName();
    }

    private void BindByIDName()
    {
        // 프리팹 이름(20001)을 바로 숫자로 변환
        if (int.TryParse(gameObject.name.Replace("(Clone)", "").Trim(), out int id))
        {
            myData = PartnerDataLoader.Instance.GetPartnerData(id);

            if (myData != null)
            {
                ApplyData();
            }
            else
            {
                Debug.LogError($"[DataBinder] ID {id}에 해당하는 데이터를 CSV에서 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError($"[DataBinder] 프리팹 이름 '{gameObject.name}'이 숫자 형식이 아닙니다.");
        }
    }

    private void ApplyData()
    {
        currentAtkDamage = myData.Atk_Damage;
        currentAtkSpeed = myData.Atk_Speed;

        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.speed = currentAtkSpeed; 
        }

        Debug.Log($"<color=cyan>[데이터 바인딩]</color> ID: {myData.ID} ({myData.Name_KR}) 연결 완료");
    }
}