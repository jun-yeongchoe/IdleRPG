using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    [SerializeField] PlayerStatus playerStatus;
    [SerializeField] StatBase atkStat;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyTest"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                float damage = atkStat.GetValue(playerStatus.atkPower);
                enemy.TakeDamage(damage);
                Debug.Log("딜 : " + damage + " 적 : " + collision.name);
            }
        }
    }

}