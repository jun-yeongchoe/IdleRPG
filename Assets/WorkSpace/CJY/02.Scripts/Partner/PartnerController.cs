using System.Collections;
using System.Collections.Generic;
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

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerController = FindObjectOfType<PlayerController>();
        Debug.Log(playerController is null);
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
        anim.SetBool("isWalking", true);
    }

    private void Attack()
    {
        anim.SetBool("isWalking", false);
        anim.SetTrigger("Attack");
    }
}
