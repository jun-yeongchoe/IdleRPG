using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[CreateAssetMenu(fileName ="EnemyData", menuName ="Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Basic")]
    public string enemyName = "Default";

    [Header("Stats")]
    public int maxHp=100;
    public float attackPower=2f;
    public float moveSpeed=1.5f;
    public float attackRange=1.5f;
    public float attackCooldown = 1.5f;
}
