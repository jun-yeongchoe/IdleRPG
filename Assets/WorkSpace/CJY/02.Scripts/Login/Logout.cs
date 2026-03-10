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
        GoogleLogin.isLogoutCalled = true;

        FirebaseAuth.DefaultInstance.SignOut();

        GoogleSignIn.DefaultInstance.SignOut();

        DBManager.Instance.userId = "";
        DBManager.Instance.userName = "";

        SceneManager.LoadScene(loginScene);
    }

    
}
