using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

public class UniversalSheetLoader : Editor
{
    // 각 시트의 GID가 포함된 최종 CSV 주소
    private const string EquipURL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRyDVW3msTQbFIC8alNBIsTJ54oBNKmlY1-RY5yF-xkKVKORx_-haqwvcTx9jHJfJ9ds5DbWlPsVigp/pub?gid=1410818889&single=true&output=csv";
    private const string SkillURL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRyDVW3msTQbFIC8alNBIsTJ54oBNKmlY1-RY5yF-xkKVKORx_-haqwvcTx9jHJfJ9ds5DbWlPsVigp/pub?gid=442380314&single=true&output=csv";
    private const string PartnerURL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRyDVW3msTQbFIC8alNBIsTJ54oBNKmlY1-RY5yF-xkKVKORx_-haqwvcTx9jHJfJ9ds5DbWlPsVigp/pub?gid=1209793167&single=true&output=csv";

    // 저장 경로 설정
    private const string BasePath = "Assets/WorkSpace/CJY/05.Data/Resources/";

    [MenuItem("Tools/Sync All Data")]
    public static void SyncAll()
    {
        SyncEquipments();
        SyncSkills();
        SyncCompanions();
    }

    [MenuItem("Tools/Sync Data/Equipments")]
    public static void SyncEquipments() => DownloadCSV(EquipURL, BasePath + "Equipments/", ParseEquip);

    [MenuItem("Tools/Sync Data/Skills")]
    public static void SyncSkills() => DownloadCSV(SkillURL, BasePath + "Skills/", ParseSkill);

    [MenuItem("Tools/Sync Data/Partners")]
    public static void SyncCompanions() => DownloadCSV(PartnerURL, BasePath + "Partners/", ParseCompanion);

    private static void DownloadCSV(string url, string path, Action<string[], string> parseAction)
    {
        Debug.Log($"다운로드 시작: {url}");
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            var op = www.SendWebRequest();
            while (!op.isDone) { }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"다운로드 실패: {www.error}");
                return;
            }

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            string[] lines = www.downloadHandler.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;
                string[] values = lines[i].Split(',');
                parseAction(values, path);
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"{path} 동기화 완료.");
    }

    // --- 각 타입별 파싱 로직 ---

    private static void ParseEquip(string[] v, string path)
    {
        int id = int.Parse(v[0]);
        var so = GetOrCreateSO<EquipmentDataSO>(path, id);
        so.ID = id;
        so.Name_KR = v[1];
        so.Name_EN = v[2];
        Enum.TryParse(v[3], true, out so.itemRank);
        Enum.TryParse(v[4], true, out so.equipmentType); // Weapon, Armor 등
        so.Value = float.Parse(v[5]);
        so.Description = v[6];
        so.BaseComposeCount = int.Parse(v[7]);
        so.BaseStatBoost = float.Parse(v[8]);
        so.StatPerLevel = float.Parse(v[9]);
        EditorUtility.SetDirty(so);
    }

    private static void ParseSkill(string[] v, string path)
    {
        int id = int.Parse(v[0]);
        var so = GetOrCreateSO<SkillDataSo>(path, id);
        so.ID = id;
        so.Name_KR = v[1];
        so.Name_EN = v[2];
        Enum.TryParse(v[3], true, out so.itemRank);
        Enum.TryParse(v[5], true, out so.skillTargetType);
        so.StrikeCount = int.Parse(v[6]);
        so.Damage_Coef = float.Parse(v[7]);
        so.Cooltime = float.Parse(v[8]);
        so.EffectPrefabName = v[9];
        so.BaseComposeCount = int.Parse(v[10]);
        so.StatPerLevel = float.Parse(v[11]);
        EditorUtility.SetDirty(so);
    }

    private static void ParseCompanion(string[] v, string path)
    {
        int id = int.Parse(v[0]);
        var so = GetOrCreateSO<PartnerDataSO>(path, id);
        so.ID = id;
        so.Name_KR = v[1];
        so.Name_EN = v[2];
        Enum.TryParse(v[3], true, out so.itemRank);
        so.AttackDamage = float.Parse(v[4]);
        so.AttackSpeed = float.Parse(v[5]); // 초당 공격 횟수
        so.BaseComposeCount = int.Parse(v[6]);
        so.StatPerLevel = float.Parse(v[7]);
        so.PrefabPath = v[8];
        so.Description = v[9];
        EditorUtility.SetDirty(so);
    }

    private static T GetOrCreateSO<T>(string path, int id) where T : ItemBase
    {
        string assetPath = $"{path}{typeof(T).Name}_{id}.asset";
        T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        if (asset == null)
        {
            asset = CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, assetPath);
        }
        return asset;
    }
}