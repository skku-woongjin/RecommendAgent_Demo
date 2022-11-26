using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class Chat
{
    public string chat;
}
public class Quest
{
    public string keywords;
}
public class request : MonoBehaviour
{
    public TMP_Text questText;
    public TMP_Text catQuestText;
    public Button QuestBtn;
    public bool ishate = false;
    public GameObject quest_panel;
    private string locationQuest;
    private string actionQuest;
    public TMP_Text currentLocation;
    private bool questIsGenerated;
    public string[] actionKeyword = { "jump", "fly", "walk", "go", "play", "find" };
    public string[] locationKeyword = { "Jurassicpark", "gamemachine", "planetland", "playground" };
    void Start()
    {
        // quest_panel.SetActive(false);
    }
    public void NewQuest()
    {
        locationQuest = locationKeyword[Random.Range(0, locationKeyword.Length)];
        actionQuest = actionKeyword[Random.Range(0, actionKeyword.Length)];
        StartCoroutine(UploadKeyword("Let's*" + actionQuest + "*" + locationQuest));
        questIsGenerated = true;
        quest_panel.SetActive(true);
    }
    public void CheckQuestSuccess()
    {
        if (questIsGenerated == true && locationQuest == currentLocation.text)
        {
            questText.text = "mission complete!";
            questIsGenerated = false;
        }
    }
    // IEnumerator getRequest(string uri)
    // {
    //     UnityWebRequest uwr = UnityWebRequest.Get(uri);
    //     yield return uwr.SendWebRequest();
    //     if (uwr.result == UnityWebRequest.Result.ConnectionError)
    //     {
    //         Debug.Log("Error While Sending: " + uwr.error);
    //     }
    //     else
    //     {
    //         Debug.Log("Received: " + uwr.downloadHandler.text);
    //     }
    // }

    public IEnumerator Upload(System.Action<bool> callback, string line)
    {
        Chat body = new Chat();
        body.chat = line;
        string bodyData = JsonUtility.ToJson(body);
        Debug.Log(bodyData);
        // var postData = System.Text.Encoding.UTF8.GetBytes(bodyData);
        var req = new UnityWebRequest("http://15.164.98.122:5000/prediction", "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(bodyData);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else
        {
            Debug.Log(req.downloadHandler.text);
            string res = req.downloadHandler.text;
            if (res.Contains("1"))
            {
                ishate = true;
            }
            else
            {
                ishate = false;
            }
            callback(ishate);
        }
    }
    public IEnumerator UploadKeyword(string line)
    {
        //line -> 보낼 데이터
        Quest body = new Quest();
        body.keywords = line;
        string bodyData = JsonUtility.ToJson(body);
        Debug.Log(bodyData);
        // var postData = System.Text.Encoding.UTF8.GetBytes(bodyData);
        var req = new UnityWebRequest("http://54.180.29.120:5000/generator", "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(bodyData);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else
        {
            string res = req.downloadHandler.text;
            questText.text = res + "\n(Keywords: " + line.Replace("*", " ") + ")";
            catQuestText.text = res;
        }


    }
}
