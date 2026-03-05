using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    private string animIsDead = "IsDead";
    public BigInteger currentHP, maxHP;
    private Animator anim;
    PlayerController pc;
    // [SerializeField] private Slider hpBar;

    void Start()
    {
        maxHP = PlayerStat.instance.hp;
        currentHP = maxHP;
        anim = GetComponent<Animator>();
        pc = GetComponent<PlayerController>();
        InvokeRepeating("RegenHP", 1f, 1f);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(50);
        }

        Debug.Log("Current HP : " + currentHP + " / " + maxHP);
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

        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("플레이어 사망");
        CancelInvoke("RegenHP");
        foreach(GameObject partner in pc.partnerSlot)
        {
            partner.SetActive(false);
        }
        anim.SetBool(animIsDead, true);
        currentHP = 0;
        CommonPopup.Instance.ShowAlert("사망!", "캐릭터가 사망했습니다.", "부활", OnclickRevival);
        
    }

    public void OnclickRevival()
    {
        Revival();
    }

    private void Revival()
    {
        currentHP = maxHP;
        anim.SetBool(animIsDead, false);
        foreach(GameObject partner in pc.partnerSlot)
        {
            partner.SetActive(true);
        }

    }
}
