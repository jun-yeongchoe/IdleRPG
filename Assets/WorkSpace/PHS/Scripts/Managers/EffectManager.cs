using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    private Dictionary<string, EffectSpawner> spawnerDict = new Dictionary<string, EffectSpawner>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        InitSpawners();
    }

    private void InitSpawners()
    {
        EffectSpawner[] children = GetComponentsInChildren<EffectSpawner>(true);

        foreach (var spawner in children)
        {
            if (string.IsNullOrEmpty(spawner.effectName))
            {
                Debug.LogWarning($"[EffectManager] 이름 없는 스포너가 있습니다: {spawner.name}");
                continue;
            }

            if (!spawnerDict.ContainsKey(spawner.effectName))
            {
                spawnerDict.Add(spawner.effectName, spawner);
            }
            else
            {
                Debug.LogWarning($"[EffectManager] 중복된 이펙트 이름: {spawner.effectName}");
            }
        }
    }

    public void PlayEffect(string name, Vector3 pos)
    {
        if (spawnerDict.TryGetValue(name, out EffectSpawner spawner))
        {
            spawner.Play(pos);
        }
        else
        {
            Debug.LogWarning($"[EffectManager] {name} 이라는 이펙트는 없습니다!");
        }
    }
}
