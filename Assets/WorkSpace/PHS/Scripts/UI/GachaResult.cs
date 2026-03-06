using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GachaResult : MonoBehaviour
{
    public Image gradeFrame;             // 등급별 색상이 바뀔 테두리 배경
    public TextMeshProUGUI itemNameText; // 아이템 이름 (현재는 ID 표시)
    public TextMeshProUGUI countText;    // 획득 개수

    public void Setup(int itemID, int count)
    {
        //텍스트 세팅
        itemNameText.text = $"ID: {itemID}";
        countText.text = $"x {count}";

        //등급 판별
        //예: 5202 (Epic 방어구) -> (5202 % 1000) = 202 -> 202 / 400 = 0 (오류)
        int categoryBase = (itemID / 1000) * 1000;
        int gradeOffset = itemID - categoryBase; //ex) 5202 - 4000 = 1202
        int grade = gradeOffset / 400;           //1202 / 400 = 3 (Epic)

        //테두리 색상 적용
        SetGradeColor(grade);
    }

    private void SetGradeColor(int grade)
    {
        //색상(0:Common ~ 6:Celestial)임시 하드코딩
        Color[] gradeColors = new Color[]
        {
            Color.white,            //Common
            Color.green,            //Uncommon
            new Color(0f, 0.5f, 1f),//Rare (파랑)
            new Color(0.8f, 0f, 1f),//Epic (보라)
            Color.yellow,           //Legendary
            Color.red,              //Mythic
            Color.cyan              //Celestial
        };

        if (grade >= 0 && grade < gradeColors.Length && gradeFrame != null)
        {
            gradeFrame.color = gradeColors[grade];
        }
    }
}
