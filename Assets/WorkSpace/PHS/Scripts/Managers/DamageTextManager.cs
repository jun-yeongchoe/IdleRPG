using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance;

    [Header("풀링 설정")]
    public GameObject damageTextPrefab; //띄울 텍스트 프리팹
    public int poolSize = 30;

    private Queue<GameObject> poolQueue = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(damageTextPrefab, transform);
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
        }
    }

    public void ShowDamage(BigInteger damage, Vector3 position)
    {
        if (poolQueue.Count == 0) return;

        GameObject obj = poolQueue.Dequeue();

        Vector3 offset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(0.5f, 1.0f), 0);
        obj.transform.position = position + offset;

        obj.SetActive(true);
        obj.GetComponent<DamageText>().Setup(damage);

        poolQueue.Enqueue(obj);
    }
}
