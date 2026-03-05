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

    public SPPointData(SPData data)
    {
        ID = data.id;
        Rank = data.rank;
        Type = data.type;
        Rate = data.rate;
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
