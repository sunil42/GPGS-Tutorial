using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
using System;
using UnityEngine.SocialPlatforms;

public class GPGSManager : MonoBehaviour
{
    public Text m_Message;
    public Button m_SignIn;
    public Button m_LoadFriendsButton;

    private LoadFriendsStatus lfs = LoadFriendsStatus.Unknown;
    private FriendsListVisibilityStatus mFriendsListVisibilityStatus = FriendsListVisibilityStatus.Unknown;
    [SerializeField] protected Transform m_FriendListContent;
    public GameObject m_FriendListPrefab;


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
        m_LoadFriendsButton.onClick.RemoveAllListeners();

        m_SignIn.onClick.AddListener(SignInGooglePlayGames);
        m_LoadFriendsButton.onClick.AddListener(LoadFriends);

        SignInGooglePlayGames();
    }

    private void LoadFriends()
    {
        //Return if not signedIn.
        if (!PlayGamesPlatform.Instance.IsAuthenticated()) {
            m_Message.text = "Please Sign In.";
            return;
        }

        LoadFriendsStatus lfs = PlayGamesPlatform.Instance.GetLastLoadFriendsStatus();

        PlayGamesPlatform.Instance.GetFriendsListVisibility( /* forceReload= */ true,
            friendsListVisibilityStatus => { mFriendsListVisibilityStatus = friendsListVisibilityStatus; });

        switch (mFriendsListVisibilityStatus)
        {
            case FriendsListVisibilityStatus.Visible:
                //Arg0 - Page Size
                PlayGamesPlatform.Instance.LoadFriends(1, false, (result) =>
                {
                    int numOfFriends = 0;

                    foreach (IUserProfile friends in Social.localUser.friends)
                    {
                        //friends.userName
                        //Create Object
                        GameObject go = Instantiate<GameObject>(m_FriendListPrefab);
                        go.transform.SetParent(m_FriendListContent, false);
                        go.GetComponent<FriendListItem>().setUp(numOfFriends, friends.userName);
                        numOfFriends++;
                    }
                });
                break;
            case FriendsListVisibilityStatus.ResolutionRequired:
                PlayGamesPlatform.Instance.AskForLoadFriendsResolution((result) => {
                    if (result == UIStatus.Valid)
                    {
                        m_Message.text = "Agree";
                    }
                    else
                    {
                        m_Message.text = result.ToString();
                    }
                });
                break;
            case FriendsListVisibilityStatus.Unknown:
                m_Message.text = "Unknow. Try Again";
                break;

        }
        Debug.Log("Load Friends Status : " + lfs.ToString());
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
