using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoGacha : MonoBehaviour
{
    public void OnClickGoGacha()
    {
        SceneManager.LoadScene("GatchaSystem_test");
    }

    public void OnClickGoMain()
    {
        SceneManager.LoadScene("Game Scene_1st");
    }
}
