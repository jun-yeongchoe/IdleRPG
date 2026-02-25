using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawner : MonoBehaviour
{
    [Header("이펙트 설정")]
    public string effectName;  //호출할 이름(예: "Hit", "Gold")
    public GameObject prefab;  //이펙트 프리팹
    public int poolSize = 20;  //풀 크기

    private Queue<GameObject> poolQueue = new Queue<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
        }
    }

    public void Play(Vector3 pos)
    {
        if (poolQueue.Count == 0) return;

        GameObject selectObj = poolQueue.Dequeue();

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
        poolQueue.Enqueue(obj);
    }
}
