using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;

public class ProfileBinder : MonoBehaviour
{
    [SerializeField] private Image profileImage;

    void Start()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

        if(user != null && user.PhotoUrl != null)
        {
            StartCoroutine(LoadProfile(user.PhotoUrl.ToString()));
        }
    }

    IEnumerator LoadProfile(string url)
    {
        UnityWebRequest www =UnityWebRequestTexture.GetTexture(url);

        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("이미지 로드 실패: " + www.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);

            profileImage.sprite = Sprite.Create(
                texture,
                new Rect(0,0,texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
            Debug.Log("프로필 이미지 로드 완료");
        }
    }
}
