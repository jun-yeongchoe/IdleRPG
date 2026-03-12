using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Google;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Logout : MonoBehaviour
{
    [SerializeField] private string loginScene = "Login Scene_1st";

    public void OnClickLogout()
    {
        DBManager.Instance.SaveSOData();

        GoogleLogin.isLogoutCalled = true;

        FirebaseAuth.DefaultInstance.SignOut();

        GoogleSignIn.DefaultInstance.SignOut();
        GoogleSignIn.DefaultInstance.Disconnect();

        DBManager.Instance.ClearData();

        SceneManager.LoadScene(loginScene);
    }

    
}
