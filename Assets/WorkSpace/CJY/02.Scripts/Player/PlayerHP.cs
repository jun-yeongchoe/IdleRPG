using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public float currentHP, maxHP;

    // [SerializeField] private Slider hpBar;

    void Start()
    {
        maxHP = PlayerStat.instance.hp;
        currentHP = maxHP;

        InvokeRepeating("RegenHP", 1f, 1f);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(50f);
        }

        Debug.Log("Current HP : " + currentHP + " / " + maxHP);
    }

    public void RefreshHP()
    {
        float hpGap = PlayerStat.instance.hp - maxHP;
        maxHP = PlayerStat.instance.hp;
        if(hpGap > 0) currentHP += hpGap;
        if(currentHP > maxHP) currentHP = maxHP;

    }

    private void RegenHP()
    {
        if(currentHP >= maxHP) return;
        currentHP = Mathf.Min(currentHP + PlayerStat.instance.hpGen, maxHP);
    }

    private void TakeDamage(float dmg)
    {
        currentHP = Mathf.Max(currentHP - dmg, 0);

        if(currentHP <= 0)
        {
            Die();
            
        }
    }

    private void Die()
    {
        Debug.Log("Player is dead!");
        currentHP = 0;
        CancelInvoke("RegenHP");
    }

}
