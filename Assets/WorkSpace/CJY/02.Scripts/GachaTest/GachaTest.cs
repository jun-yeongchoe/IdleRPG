using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GachaTest : MonoBehaviour
{
    [Header("UI")]
    public TMP_Dropdown levelChoose;
    public TextMeshProUGUI[] result;
    public Button summon11Btn, summon35Btn;
    private DatabaseReference gachaRef;

    private DatabaseReference dbRef;
    private Dictionary<string, float> currentRates = new Dictionary<string, float>();
    private string[] rankKeys = { "Common", "Uncommon", "Rare", "Epic", "Legendary", "Mythic", "Celestial" };

    void Start()
    {
        levelChoose.ClearOptions();

        List<string> options = new List<string>();
        for(int i = 1; i <= 20; i++)
        {
            options.Add(i.ToString());
        }

        levelChoose.AddOptions(options);

        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        gachaRef = dbRef.Child("LevelUpManagement").Child("SummonSettings").Child("Gacha");

        summon11Btn.onClick.AddListener(() => DoSummon(11));
        summon35Btn.onClick.AddListener(() => DoSummon(35));

       levelChoose.onValueChanged.AddListener
       (
        delegate 
        { 
            StartCoroutine(LoadRatesFromDB()); 
        }
        );

        StartCoroutine(LoadRatesFromDB());
    }

    IEnumerator LoadRatesFromDB()
    {
        string selectedLevel = $"Level{levelChoose.options[levelChoose.value].text}";
        var task = gachaRef.Child(selectedLevel).GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.Log("파베 로드 실패");
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            currentRates.Clear();

            foreach(var key in rankKeys)
            {
                if (snapshot.HasChild(key))
                {
                    currentRates.Add(key, float.Parse(snapshot.Child(key).Value.ToString()));
                }
            }
            Debug.Log($"{selectedLevel} 확률 데이터 로드 완료");
        }
    }

    private void DoSummon(int count)
    {
        if(currentRates.Count == 0)return;

        int[] results = new int[7];

        for(int i = 0; i<count; i++)
        {
            int idx = GetRandRankIdx();
            results[idx]++;
        }
        for(int i = 0; i< result.Length; i++)
        {
            result[i].text = results[i].ToString();
        }
    }

    private int GetRandRankIdx()
    {
        float totalWeight = 0;

        foreach(var rate in currentRates.Values) totalWeight += rate;

        float randValue = Random.Range(0, totalWeight);
        float currentWeight = 0;

        for(int i = 0; i< rankKeys.Length; i++)
        {
            currentWeight += currentRates[rankKeys[i]];
            if(randValue <= currentWeight) return i;

        }

        return 0;

    }

    public void OnClickBackStatusWindow()
    {
        SceneManager.LoadScene("LoginTest");
    }

}
