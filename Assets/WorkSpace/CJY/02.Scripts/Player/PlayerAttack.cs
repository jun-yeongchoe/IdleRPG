using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Animator anim;

    private float attackDelayDenominator = 1f;
    private float lastAttackTime = 0f;
    [SerializeField] PlayerStatus playerStatus;

    [Header("Attack Damage")]
    [SerializeField] PolygonCollider2D weaponCollider;

    public void EnableWeaponCollider()
    {
        weaponCollider.enabled = true;
    }

    public void DisableWeaponCollider()
    {
        weaponCollider.enabled = false;
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        
    }

    public void Attack()
    {
        float attackInterval = attackDelayDenominator / playerStatus.atkSpeed; 
        Debug.Log("Attack Interval: " + attackInterval);
        Debug.Log("Attack Speed: " + playerStatus.atkSpeed);
        if (Time.time - lastAttackTime > attackInterval)
        {
            anim.SetFloat("attackSpeed", playerStatus.atkSpeed);
            anim.SetTrigger("swing");
            lastAttackTime = Time.time;
        }
    }
    
    public void TakeDamage()
    {
        
    }
  
}
