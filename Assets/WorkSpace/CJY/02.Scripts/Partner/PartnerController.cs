using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PartnerController : MonoBehaviour
{
    public enum State
    {
        Walk,
        Attack
    }

    [Header("State")]
    public State currentState = State.Walk;
    private Animator anim;
    private PlayerController playerController;
    private PartnerDataBinder partnerStat;
    private float lastAttackTime = 0f;

    [SerializeField] private Transform weapon;

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerController = FindObjectOfType<PlayerController>();
        partnerStat = GetComponent<PartnerDataBinder>();
        Debug.Log(playerController is null);
        if(weapon == null) Debug.Log("무기가 없는 객체임");
    }

    private void FixedUpdate()
    {
        if (playerController == null) return;

        if (playerController.isCombat)
        {
            Attack();
        }
        else
        {
            Walk();
        }
    }


    private void Walk()
    {
        if(currentState == State.Walk) return;

        currentState = State.Walk;
        anim.SetBool("isWalking", true);
        if(weapon != null) weapon.gameObject.SetActive(false);
    }

    private void Attack()
    {
        currentState = State.Attack;
        anim.SetBool("isWalking", false);
        if(weapon != null) weapon.gameObject.SetActive(true);
        
        float attackInterval = 1f/(partnerStat != null? partnerStat.currentAtkSpeed : 1f);

        if(Time.time - lastAttackTime >= attackInterval)
        {
           anim.SetTrigger("Attack");
           ActiveSkill();
           lastAttackTime = Time.time;
        }
    }

    private void ActiveSkill()
    {
        EnemyBase target = playerController.GetCurrentTarget();

        if(target != null)
        {
            EffectManager.Instance.PlayEffect("EarthBeam", target.transform.position);
            BigInteger damage = (partnerStat != null)? partnerStat.currentAtkDamage : 0;
            target.TakeDamage(damage);

            Debug.Log($"[Partner] {gameObject.name}가 {target.name}에게 {damage} 의 피해를 입힘.");
        }
    }

}
