// using System;
// using System.IO;
// using UnityEngine;
// using UnityEditor;
// using UnityEngine.Networking;

// [InitializeOnLoad]
// public class SkillLoaderFromGoogleSheets
// {
//     private const string csvURL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRyDVW3msTQbFIC8alNBIsTJ54oBNKmlY1-RY5yF-xkKVKORx_-haqwvcTx9jHJfJ9ds5DbWlPsVigp/pub?gid=442380314&single=true&output=csv";
//     private const string savePath = "Assets/WorkSpace/CJY/05.Data/Skills/";

//     static SkillLoaderFromGoogleSheets()
//     {
//         EditorApplication.playModeStateChanged += (state) =>
//         {
//             if (state == PlayModeStateChange.ExitingEditMode) DownloadCSV();
//         };
//     }

//     [MenuItem("Tools/Sync Skill Data")]
//     public static void DownloadCSV()
//     {
//         Debug.Log("구글 시트에서 스킬 데이터 동기화 시작...");

//         using (UnityWebRequest www = UnityWebRequest.Get(csvURL))
//         {
//             var operation = www.SendWebRequest();
//             while (!operation.isDone) { }

//             if (www.result != UnityWebRequest.Result.Success)
//             {
//                 Debug.LogError("CSV 다운로드 실패: " + www.error);
//             }
//             else
//             {
//                 ParseCSV(www.downloadHandler.text);
//                 Debug.Log("스킬 SO 생성 및 업데이트 완료.");
//             }
//         }
//     }

//     private static void ParseCSV(string text)
//     {
//         if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

//         // 줄바꿈 기호 대응 (\r\n 또는 \n)
//         string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

//         for (int i = 1; i < lines.Length; i++)
//         {
//             if (string.IsNullOrWhiteSpace(lines[i])) continue;

//             string[] values = lines[i].Split(',');
            
//             // 데이터 매핑 (CSV 컬럼 순서 주의!)
//             // 0:ID, 1:Name_KR, 2:Name_EN, 3:Rank, 4:Type, 5:Strike_Count, 6:Damage_Coef, 7:Cooltime, 8:Effect_Prefab, 9:Splash_Radius
//             int id = int.Parse(values[0]);
//             string assetPath = $"{savePath}Skill_{id}.asset";
//             SkillDataSO asset = AssetDatabase.LoadAssetAtPath<SkillDataSO>(assetPath);

//             if (asset == null)
//             {
//                 asset = ScriptableObject.CreateInstance<SkillDataSO>();
//                 AssetDatabase.CreateAsset(asset, assetPath);
//             }

//             // 데이터 입력 시작
//             asset.ID = id;
//             asset.Name_KR = values[1].Trim();
//             asset.Name_EN = values[2].Trim();

//             // Enum 변환 (대소문자 무시 옵션 true)
//             if (Enum.TryParse(values[3].Trim(), true, out Rank parsedRank))
//                 asset.rank = parsedRank;
            
//             if (Enum.TryParse(values[4].Trim(), true, out Type parsedType))
//                 asset.type = parsedType;

//             asset.Strike_Count = int.Parse(values[5]);
//             asset.Damage_Coef = float.Parse(values[6]);
//             asset.Cooltime = float.Parse(values[7]);
//             asset.Effectprefab = values[8].Trim();

//             // Splash_Radius 데이터가 시트에 있다면 파싱 (없으면 기본값 2 사용)
//             if (values.Length > 9 && !string.IsNullOrEmpty(values[9]))
//             {
//                 asset.Splash_Radius = float.Parse(values[9]);
//             }

//             EditorUtility.SetDirty(asset);
//         }

//         AssetDatabase.SaveAssets();
//         AssetDatabase.Refresh();
//     }
// }