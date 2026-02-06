using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [SerializeField] PlayerStatus playerStatus;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Player Stat Data:");
            Debug.Log("Attack Power Level: " + playerStatus.atkPower);
            Debug.Log("HP Level: " + playerStatus.hp);
            Debug.Log("HP Regen Level: " + playerStatus.hpGen);
            Debug.Log("Attack Speed Level: " + playerStatus.atkSpeed);
            Debug.Log("Critical Chance Level: " + playerStatus.criticalChance);
            Debug.Log("Critical Damage Level: " + playerStatus.criticalDamage);
        }

        if(Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("User Info:");
            Debug.Log("Attack Power: " + playerStatus.GetAtkPower());
            Debug.Log("HP: " + playerStatus.GetHP());
            Debug.Log("HP Regen: " + playerStatus.GetHPGen());
            Debug.Log("Attack Speed: " + playerStatus.GetAtkSpeed());
            Debug.Log("Critical Chance: " + playerStatus.GetCriticalChance());
            Debug.Log("Critical Damage: " + playerStatus.GetCriticalDamage());
        }
    }
}
