using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum type
{
    Equipment,
    Skill,
    Partner
}

public class ItemPrefabGenerator : EditorWindow
{
    [Serializable]
    public struct SheetInfo
    {
        public type typeName;
        public string url;
        public string savePath;
    }

    private List<SheetInfo> sheets = new List<SheetInfo>()
    {
        new SheetInfo
        {
            typeName = type.Equipment,
            url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRyDVW3msTQbFIC8alNBIsTJ54oBNKmlY1-RY5yF-xkKVKORx_-haqwvcTx9jHJfJ9ds5DbWlPsVigp/pub?gid=1410818889&single=true&output=csv",
            savePath = "Assets/WorkSpace/CJY/07.Prefabs/Resources/Items/EquipItem/"
        },
        new SheetInfo
        {
            typeName = type.Skill,
            url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRyDVW3msTQbFIC8alNBIsTJ54oBNKmlY1-RY5yF-xkKVKORx_-haqwvcTx9jHJfJ9ds5DbWlPsVigp/pub?gid=442380314&single=true&output=csv",
            savePath = "Assets/WorkSpace/CJY/07.Prefabs/Resources/Items/Skills/"
        },
        new SheetInfo
        {
            typeName = type.Partner,
            url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRyDVW3msTQbFIC8alNBIsTJ54oBNKmlY1-RY5yF-xkKVKORx_-haqwvcTx9jHJfJ9ds5DbWlPsVigp/pub?gid=1209793167&single=true&output=csv",
            savePath = "Assets/WorkSpace/CJY/07.Prefabs/Resources/Items/Partners/"
        }
    };

    [MenuItem("Tools/Generate All Item Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<ItemPrefabGenerator>("All Prefab Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("CSV 통합 프리팹 생성기", EditorStyles.boldLabel);
        if(GUILayout.Button("전체 프리팹 생성 시작(Equip/Skill/Partner)"))
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            EditorCoroutineRunner.StartCoroutine(ProcessAllSheets());
        }
    }

    IEnumerator ProcessAllSheets()
    {
        foreach(var sheet in sheets)
        {
            Debug.Log($"{sheet.typeName} 데이터 다운로드 중..");

            using(UnityWebRequest www = UnityWebRequest.Get(sheet.url))
            {
                yield return www.SendWebRequest();

                if(www.result  != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"{sheet.typeName} 다운로드 실패!");
                    Debug.LogError($"Error: {www.error}");
                    Debug.LogError($"Response Code: {www.responseCode}");
                    Debug.LogError($"Result: {www.result}");
                    continue;
                }
                ParseAndCreatePrefabs(www.downloadHandler.text, sheet);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("완료", "프리팹 생성 완료되었습니다.","확인");
    }

    private void ParseAndCreatePrefabs(string text, SheetInfo sheet)
    {
        string[] lines = text.Split('\n');
        if(!Directory.Exists(sheet.savePath)) Directory.CreateDirectory(sheet.savePath);

        string spritePath = @"Assets\import\HONETi\fantasy_gui_4\_4K\textures\atlas\fg4_buttons_02.psd";
        Sprite loadedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

        for(int i = 1; i < lines.Length; i++)
        {
            if(string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');

            string id = values[0].Trim();
            string nameKr= values[1].Trim();
            string iconPath = values[2].Trim();
            string rank = values[3].Trim();

            string fileName = $"{id}";
            string localPath = $"{sheet.savePath}{fileName}.prefab";

            GameObject go = new GameObject(fileName);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100,100);
            Image img = go.AddComponent<Image>();
            img.color = GetRankColor(rank);
            if (loadedSprite != null) img.sprite = loadedSprite;

            GameObject child = new GameObject("IconImage");
            child.transform.SetParent(go.transform);

            RectTransform childRect = child.AddComponent<RectTransform>();
            childRect.anchoredPosition = Vector2.zero;
            childRect.sizeDelta = new Vector2(80,80);

            Image childImg = child.AddComponent<Image>();
            childImg.preserveAspect = true;

            string baseIconFolder = @"Assets\import\ItemIcon";
            string fullIconPath = "";

            if(sheet.typeName == type.Equipment)
            {
                fullIconPath = Path.Combine(baseIconFolder, "Equipment", $"{iconPath}.png");
            }
            else
            {
                fullIconPath = Path.Combine(baseIconFolder, sheet.typeName.ToString(),$"{id}.png");
            }

            fullIconPath = fullIconPath.Replace("\\", "/");
            Sprite iconSprite = AssetDatabase.LoadAssetAtPath<Sprite>(fullIconPath);

            if (iconSprite != null)
            {
                childImg.sprite = iconSprite;
            }
            else
            {
                Debug.LogWarning($"<color=red>[아이콘 누락]</color> 경로 확인 필요: {fullIconPath}");
            }

            ItemRank itemRank = (ItemRank)Enum.Parse(typeof(ItemRank), rank, true);

            switch (sheet.typeName)
            {
                case type.Equipment:
                    var equip = go.AddComponent<EquipmentDataSO>();
                    SetBaseInfo(equip, values, itemRank, ItemType.Equipment);

                    equip.equipmentType = (EquipmentType)System.Enum.Parse(typeof(EquipmentType), values[4].Trim());
                    equip.Value = float.Parse(values[5].Trim());
                    equip.Description = values[6].Trim();
                    equip.BaseComposeCount = int.Parse(values[7].Trim());
                    equip.BaseStatBoost = float.Parse(values[8].Trim());
                    equip.StatPerLevel = float.Parse(values[9].Trim());
                    break;

                case type.Skill:
                    var skill = go.AddComponent<SkillDataSo>();
                    SetBaseInfo(skill, values, itemRank, ItemType.Skill);
                    
                    skill.skillTargetType = (SkillTargetType)Enum.Parse(typeof(SkillTargetType), values[5].Trim());
                    skill.StrikeCount = int.Parse(values[6].Trim());
                    skill.Damage_Coef = float.Parse(values[7].Trim());
                    skill.Cooltime = float.Parse(values[8].Trim());
                    skill.EffectPrefabName = values[9].Trim();
                    skill.BaseComposeCount = int.Parse(values[10].Trim());
                    skill.StatPerLevel = float.Parse(values[11].Trim());
                    skill.spawnPoint = (SkillSpawnPoint)Enum.Parse(typeof(SkillSpawnPoint), values[12].Trim());
                break;

                case type.Partner:
                    var partner = go.AddComponent<PartnerDataSO>();
                    SetBaseInfo(partner, values, itemRank, ItemType.Partner);
                    
                    partner.AttackDamage = float.Parse(values[4].Trim());
                    partner.AttackSpeed = float.Parse(values[5].Trim());
                    partner.BaseComposeCount = int.Parse(values[6].Trim());
                    partner.StatPerLevel = float.Parse(values[7].Trim());
                    partner.PrefabPath = values[8].Trim();
                    partner.Description = values[9].Trim();
                break;
            }

            PrefabUtility.SaveAsPrefabAsset(go, localPath);
            DestroyImmediate(go);

            Debug.Log($"<color=yellow>[{sheet.typeName}]</color> 생성 완료: {fileName}");
        }
    }

    //공통 필드 세팅용 헬퍼함수
    private void SetBaseInfo(ItemBase item, string[] values, ItemRank rank, ItemType type)
    {
        item.ID = int.Parse(values[0]);
        item.Name_KR = values[1].Trim();
        item.Name_EN = values[2].Trim();
        item.itemRank = rank;
        item.itemType = type;
    }

    private Color GetRankColor(string rank)
    {
        if (rank.Contains("Common")) return Color.white;
        if (rank.Contains("Uncommon")) return new Color(0.2f, 1f, 0.2f);
        if (rank.Contains("Rare")) return Color.blue;
        if (rank.Contains("Epic")) return new Color(0.6f, 0f, 1f);
        if (rank.Contains("Legendary")) return Color.yellow;
        if (rank.Contains("Mythic")) return Color.red;
        if (rank.Contains("Celestial")) return Color.cyan;
        return Color.gray;
    }
    
    public static class EditorCoroutineRunner
    {
        public static void StartCoroutine(IEnumerator update)
        {
            EditorApplication.CallbackFunction closure = null;
            closure = () => 
            {
                // 현재 IEnumerator가 무엇을 yield 하고 있는지 확인
                object current = update.Current;

                // 만약 유니티 웹 요청 같은 AsyncOperation을 기다리는 중이라면
                if (current is AsyncOperation op && !op.isDone)
                {
                    return; // 아직 작업 중이므로 다음 업데이트까지 대기
                }

                // 작업이 끝났거나 일반 yield return null인 경우 진행
                if (!update.MoveNext()) 
                {
                    EditorApplication.update -= closure;
                }
            };
            EditorApplication.update += closure;
        }
    }
}
