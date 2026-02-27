using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetailInfoClose : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    public void OnClickClose()
    {
        panel.SetActive(false);
    }
}
