using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;

public class DamageText : MonoBehaviour
{
    [Header("연출 설정")]
    public float moveSpeed = 2.0f;    //위로 올라가는 속도
    public float alphaSpeed = 3.0f;   //투명해지는 속도
    public float destroyTime = 1.0f;  //화면에 머무는 시간

    private TextMeshPro textMesh;     //UI가 아닌 World Space용 텍스트
    private Color alpha;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(BigInteger damage)
    {
        StopAllCoroutines();
        textMesh.text = damage.ToString();

        alpha = textMesh.color;
        alpha.a = 1f;
        textMesh.color = alpha;

        StartCoroutine(FloatingAndFade());
    }

    private IEnumerator FloatingAndFade()
    {
        float time = 0;

        while (time < destroyTime)
        {
            transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));

            alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
            textMesh.color = alpha;

            time += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
