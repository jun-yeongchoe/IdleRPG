using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement settings")]
    public float moveSpeed = 0.005f;

    Animator anim;
    Rigidbody2D rb;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move()
    {
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        anim.SetBool("IsWalk", true);
    }

    public void Stop()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetBool("IsWalk", false);
    }
}
