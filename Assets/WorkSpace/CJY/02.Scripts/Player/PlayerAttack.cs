using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Animator anim;
    public float attackDelay = 0.5f;
    private float lastAttackTime = 0f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    
    public void Attack()
    {
        if (Time.time - lastAttackTime > attackDelay)
        {
            anim.SetTrigger("swing");
            lastAttackTime = Time.time;
        }
    }
  
}
