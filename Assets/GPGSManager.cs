using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using GooglePlayGames.BasicApi;
using GooglePlayGames;

public class GPGSManager : MonoBehaviour
{
    public Text m_Message;
    public Button m_SignIn;


    private void Start()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
    .RequestIdToken()
    .RequestServerAuthCode(false)
    .Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        m_SignIn.onClick.RemoveAllListeners();
        m_SignIn.onClick.AddListener(SignInGooglePlayGames);

        SignInGooglePlayGames();
    }

    private void SignInGooglePlayGames()
    {
        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptAlways, (result) =>
        {
            m_Message.text = result.ToString();
            m_SignIn.onClick.AddListener(SignoutGooglePlay);
        });
    }

    private void SignoutGooglePlay()
    {
        PlayGamesPlatform.Instance.SignOut();
        m_Message.text = "Sign Out";
        SignInGooglePlayGames();
    }
}
