using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    [SerializeField] PlayerStatus playerStatus;
    [SerializeField] StatBase atkStat;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyTest"))
        {
            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                // float damage = atkStat.GetValue(playerStatus.atkPower);
                BigInteger damage = PlayerStat.instance.atkPower;
                // enemy.TakeDamage(damage); -> BigInteger로 바꾸고나서 활성화 예정
                Debug.Log("딜 : " + damage + " 적 : " + collision.name);
            }
        }
    }

}