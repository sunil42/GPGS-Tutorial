using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MyAchievement : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Description;
    [SerializeField] private RawImage m_Icon;
    [SerializeField] private TextMeshProUGUI m_Progress;

    public void SetUpUI(string imgURL, string name, string description, double percentageComplete, bool hidden)
    {
        StartCoroutine(DownloadImage(imgURL));

        m_Title.text = name;
        if (hidden)
        {
            m_Description.text = string.Empty;
        }
        if (percentageComplete == 100.0f)
        {
            m_Progress.text = "Completed";
        } else
        {
            if (hidden)
            {
                m_Progress.text = string.Empty;
            } 
            else
            {
                m_Progress.text = percentageComplete.ToString() + " % Completed";
            }
        }
    }

    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            //Sprite sprite = Sprite.Create(imgURL, new Rect(0, 0, imgURL.width, imgURL.height), new Vector2(imgURL.width / 2, imgURL.height / 2));
            m_Icon.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }
}