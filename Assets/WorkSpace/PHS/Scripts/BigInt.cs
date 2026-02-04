using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Numerics;

public static class BigInt
{
    //1000단위로 A, B, C...식으로 단위 바뀌게 구현

    private static string[] curren = 
        {
        "","A","B","C","D","E","F","G","H","I",
        "J","K","L","M","N","O","P","Q","R","S",
        "T","U","V","W","X","Y","Z" 
    };

    public static string ToCurren(this BigInteger number) 
    {
        if(number<1000)return number.ToString();

        string numStr=number.ToString();
        int len=numStr.Length;
        int index = (len - 1) / 3;

        if (index >= curren.Length) index = curren.Length - 1;

        int frontLen = len % 3;
        if(frontLen==0)frontLen = 3;

        string head=numStr.Substring(0, frontLen);
        string tail=numStr.Substring(frontLen,2);

        return $"{head}.{tail}{curren[index]}";
    }
}
