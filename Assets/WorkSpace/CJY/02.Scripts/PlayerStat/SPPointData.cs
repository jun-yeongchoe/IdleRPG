using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SPPointData
{
    public int ID;
    public string Rank;
    public string Type;
    public float Rate;
    public bool IsMultiply;

    public SPPointData(int id, string rank, string type, float rate,  bool isMultiply)
    {
        ID = id;
        Rank = rank;
        Type = type;
        Rate = rate;
        IsMultiply = isMultiply;
    }

    public float GetSPPointBonus(string type, string rank)
    {
        if(Type != type || Rank != rank)
        {
            return 0f;
        }
        else return Rate;
    }
}
