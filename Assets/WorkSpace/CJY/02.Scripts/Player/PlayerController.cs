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
    RaycastHit2D hit;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    void FixedUpdate()
    {
        hit = Physics2D.Raycast(transform.position, Vector2.right, attackRange, enemyLayer);
        // 이동중에는 계속 쏘더라도 전투중에는 레이를 꺼놨다가 몬스터가 죽었을 때, 다시 한프레임정도켜서 확인하고 몬스터가 남아있으면 다시끄고 켜고 반복.

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
