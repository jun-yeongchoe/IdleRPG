using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSV_LoadManager : MonoBehaviour
{
    public static CSV_LoadManager Instance{get; private set;}

    public PlayerStatLoaderFromGoogleSheets playerStats_CSV;
    public SPPointCSVLoader SP_CSV;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
