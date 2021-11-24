using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendListItem : MonoBehaviour
{
    public int id = 0;

    public Text m_FriendName;
    public Button m_ShowProfileButton;

    private string mFriendId = "";

    public void setUp(int id, string friendName) 
    {
        m_FriendName.text = friendName;

        mFriendId = Social.localUser.friends[id].id;

        m_ShowProfileButton.onClick.RemoveAllListeners();
        m_ShowProfileButton.onClick.AddListener(showProfile);
    }

    private void showProfile()
    {
        PlayGamesPlatform.Instance.ShowCompareProfileWithAlternativeNameHintsUI(mFriendId, null, null,(result) => {
            Debug.Log("Closed : " + result.ToString());
        });
    }
}
