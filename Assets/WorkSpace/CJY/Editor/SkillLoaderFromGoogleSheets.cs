using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using Unity.VisualScripting;
using System;
using System.IO;


[InitializeOnLoad]
public class SkillLoaderFromGoogleSheets
{
    private const string csvURL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRyDVW3msTQbFIC8alNBIsTJ54oBNKmlY1-RY5yF-xkKVKORx_-haqwvcTx9jHJfJ9ds5DbWlPsVigp/pub?gid=442380314&single=true&output=csv";
    private const string savePath = "Assets/WorkSpace/CJY/05.Data/Skills/";

    
   static SkillLoaderFromGoogleSheets()
    {
        EditorApplication.playModeStateChanged += (state) =>
        {
            if(state == PlayModeStateChange.ExitingEditMode) DownloadCSV();
        };
    }
    

    [MenuItem("Tools/Sync Skill Data")]
    public static void DownloadCSV()
    {
        Debug.Log("구글 시트에서 스킬 데이터 동기화 시작...");

        using (UnityWebRequest www = UnityWebRequest.Get(csvURL))
        {
            // 에디터에서는 코루틴 yield return을 쓸 수 없으므로 동기적으로 기다립니다.
            var operation = www.SendWebRequest();
            while (!operation.isDone) { } 

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("CSV 다운로드 실패: " + www.error);
            }
            else
            {
                ParseCSV(www.downloadHandler.text);
                Debug.Log("스킬 SO 생성 및 업데이트 완료.");
            }
        }
    }

    private static void ParseCSV(string text)
    {
        if(!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

        string[] lines = text.Split('\n');

        for(int i = 1; i < lines.Length; i++)
        {
            if(string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] values = lines[i].Split(',');
            int id = int.Parse(values[0]);

            string assetPath = $"{savePath}Skill_{id}.asset";
            SkillDataSO asset = AssetDatabase.LoadAssetAtPath<SkillDataSO>(assetPath);

            if(asset == null)
            {
                asset = ScriptableObject.CreateInstance<SkillDataSO>();
                AssetDatabase.CreateAsset(asset, assetPath);
            }

            asset.ID = id;
            asset.Name_KR = values[1];
            asset.Name_EN = values[2];
            asset.Rank = values[3];
            asset.Skill_Type = values[4];
            asset.Damage_Coef = float.Parse(values[5]);
            asset.Cooltime = float.Parse(values[6]);
            asset.Effect_Prefab = values[7];

            EditorUtility.SetDirty(asset);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
