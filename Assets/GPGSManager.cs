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

    [SerializeField] private GameObject m_AchievementListScollView;
    [SerializeField] private Button m_ShowCustomAchievementButton;
    [SerializeField] private GameObject m_AchievementPrefab;
    [SerializeField] private GameObject m_AchievementsToInstantiateAt;

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
        Debug.Log("Signing.....");
        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (result) =>
        {
            string msg = "";
            string playerId = "";
            switch(result)
            {
                case SignInStatus.AlreadyInProgress:
                    msg = "Progress";break;
                case SignInStatus.Canceled: msg = "Canceled"; break;
                case SignInStatus.DeveloperError: msg = "Developer Error";break;
                case SignInStatus.Failed: msg = "Failed"; break;
                case SignInStatus.InternalError: msg = "Internal Error"; break;
                case SignInStatus.NetworkError: msg = "Network Error"; break;
                case SignInStatus.NotAuthenticated: msg = "Not authenticated"; break;
                case SignInStatus.Success: msg = "Success."; playerId = PlayGamesPlatform.Instance.GetUserDisplayName(); break;
                case SignInStatus.UiSignInRequired: msg = "UiSignInRequired"; break;
            }
            m_Message.text = msg + playerId;
            m_SignIn.onClick.AddListener(SignoutGooglePlay);
        });
    }

    private void SignoutGooglePlay()
    {
        PlayGamesPlatform.Instance.SignOut();
        m_Message.text = "Sign Out";
        SignInGooglePlayGames();
    }

    public void WinARace()
    {
        PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_won_a_race, 100.0f, (result) => { 
            if (result)
            {
                Debug.Log("Progress Reported");
            } else
            {
                Debug.LogWarning("Failed to report progress !!");
            }
        });
    }

    public void IncrementProgress()
    {
        PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_top_5, 10, (result) => { 
            if (result)
            {
                Debug.Log("Progress Incremented");
            }
            else
            {
                Debug.Log("Failed to increment progress !!");
            }
        });
    }

    public void RevelHiddenReward()
    {
        PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_hidden_reward, 100.0f, (result) => {
            if (result)
            {
                Debug.Log("Hidden Progress Reported");
            }
            else
            {
                Debug.LogWarning("Failed to report hidden progress !!");
            }
        });
    }

    public void ShowAchievementUI()
    {
        PlayGamesPlatform.Instance.ShowAchievementsUI();
    }

    public void ShowCustomAchievement()
    {
        m_AchievementListScollView.SetActive(!m_AchievementListScollView.activeSelf);
        if (m_AchievementListScollView.activeSelf)
        {
            m_ShowCustomAchievementButton.GetComponentInChildren<Text>().text = "Hide Custom Achievement UI";
        } else
        {
            m_ShowCustomAchievementButton.GetComponentInChildren<Text>().text = "Show Custom Achievement UI";
        }

        foreach (Transform child in m_AchievementsToInstantiateAt.transform)
        {
            Destroy(child.gameObject);
        }

        PlayGamesPlatform.Instance.LoadAchievements(achievements =>
        {
            if (achievements.Length > 0)
            {
                foreach (IAchievement achievement in achievements)
                {
                    PlayGamesAchievement playGamesAchievement = (PlayGamesAchievement) achievement;

                    GameObject achievementObj = Instantiate(m_AchievementPrefab, m_AchievementsToInstantiateAt.transform, false);
                    MyAchievement myAchievement = achievementObj.GetComponent<MyAchievement>();

                    myAchievement.SetUpUI(playGamesAchievement.ImageURL(), playGamesAchievement.title,
                        playGamesAchievement.unachievedDescription, playGamesAchievement.percentCompleted,
                        playGamesAchievement.hidden);


                    /*Debug.Log("*******MY ACIEVEMENT" + gg.currentSteps);

                    myAchievements += "\t" +
                        achievement.id + " " +
                        achievement.percentCompleted + " " +
                        achievement.completed + " " +
                        achievement.lastReportedDate + "\n";*/
                }
            }
        });
    }
}
