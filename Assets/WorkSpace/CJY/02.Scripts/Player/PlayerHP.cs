using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    private string animIsDead = "Die";
    public BigInteger currentHP, maxHP;
    private Animator anim;
    PlayerController pc;

    void Start()
    {
        EventManager.Instance.StartList("PlayerStatChange", OnStatChanged);

        anim = GetComponent<Animator>();
        pc = GetComponent<PlayerController>();
        InvokeRepeating("RegenHP", 1f, 1f);

        OnStatChanged();
    }

    void OnDestroy()
    {
        EventManager.Instance.StopList("PlayerStatChange", OnStatChanged);
    }
    private void OnStatChanged()
    {
        if(maxHP <= 0)
        {
            maxHP = PlayerStat.instance.hp;
            currentHP = maxHP;
        }
        else
        {
            RefreshHP();
        }
    }

    void Update()
    {
        transform.rotation = UnityEngine.Quaternion.Euler(0,0,0);
        if(Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(50);
        }

    }

    public void RefreshHP()
    {
        BigInteger hpGap = PlayerStat.instance.hp - maxHP;
        maxHP = PlayerStat.instance.hp;
        if(hpGap > 0) currentHP += hpGap;
        if(currentHP > maxHP) currentHP = maxHP;

    }

    private void RegenHP()
    {
        if(currentHP >= maxHP) return;
        currentHP += (BigInteger)PlayerStat.instance.hpGen;
        if (currentHP > maxHP) currentHP = maxHP;
    }

    public void TakeDamage(BigInteger dmg)
    {
        currentHP -= dmg;
        Debug.Log($"[PlayerHP]현재체력 : {currentHP}/{maxHP}");
        float randomX = UnityEngine.Random.Range(0.1f, 0.3f);
        float randomY = UnityEngine.Random.Range(0.6f, 0.8f);
        DamageTextManager.Instance.ShowDamage(dmg, this.gameObject.transform.position + new UnityEngine.Vector3(randomX,randomY,0));

        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("플레이어 사망");
        PlayerStat.instance.isDead = true;
        CancelInvoke("RegenHP");
        foreach(GameObject partner in pc.partnerSlot)
        {
            partner.SetActive(false);
        }
        anim.SetTrigger(animIsDead);
        currentHP = 0;
        StartCoroutine(ShowPopupAfterAnimation());
    }

    public void OnclickRevival()
    {
        Revival();
    }

    private void Revival()
    {
        Time.timeScale = 1;
        DataManager.Instance.currentStageNum = 1;
        PlayerStat.instance.isDead = false;
        currentHP = maxHP;
        anim.speed = 1;
        anim.SetBool(animIsDead, false);
        InvokeRepeating("RegenHP", 1f, 1f);

        foreach(GameObject partner in pc.partnerSlot)
        {
            partner.SetActive(true);
        }
    }

    IEnumerator ShowPopupAfterAnimation()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float waitTime = stateInfo.length * 0.7f;
        yield return new WaitForSeconds(waitTime);
        anim.speed = 0;
        Time.timeScale = 0;

        CommonPopup.Instance.ShowAlert("사망!", "캐릭터가 사망했습니다.", "부활", OnclickRevival);
    }
}
