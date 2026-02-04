using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMove playerMove;
    private PlayerAttack playerAttack;

    public float attackRange = 0.3f;
    bool isCombat = false;
    public LayerMask enemyLayer;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, attackRange, enemyLayer);

        if(hit.collider != null)
        {
            isCombat = true;
            playerMove.Stop();
            playerAttack.Attack();
        }
        else
        {
            isCombat = false;
            playerMove.Move();
        }
    
        Debug.DrawRay(transform.position, Vector2.right * attackRange, Color.red);

    }


}
