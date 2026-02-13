using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawner : MonoBehaviour
{
    [Header("이펙트 설정")]
    public string effectName;  //호출할 이름(예: "Hit", "Gold")
    public GameObject prefab;  //이펙트 프리팹
    public int poolSize = 20;  //풀 크기

    private GameObject[] pool;

    private void Awake()
    {
        pool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = Instantiate(prefab, transform);
            pool[i].SetActive(false);
        }
    }

    public void Play(Vector3 pos)
    {
        // 꺼진 놈 찾기
        GameObject selectObj = null;
        for (int i = 0; i < poolSize; i++)
        {
            if (!pool[i].activeSelf)
            {
                selectObj = pool[i];
                break;
            }
        }

        if (selectObj == null) return;

        selectObj.transform.position = pos;
        selectObj.SetActive(true);

        StartCoroutine(DisableCo(selectObj));
    }

    IEnumerator DisableCo(GameObject obj)
    {
        float duration = 1.0f;

        ParticleSystem ps = obj.GetComponent<ParticleSystem>();
        if (ps != null) duration = ps.main.duration;

        yield return new WaitForSeconds(duration);

        obj.SetActive(false);
    }
}
