using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Linq;


public class FB_ScoreController : MonoBehaviour
{
    DatabaseReference mDatabase;
    public Text scoreTexto;
    public static int playerScore;
    public int highestScore;
    public Dictionary<string, object> userObject;
    public Text leadersText;
    public GameObject tablaLeadersPanel;
    string UserId;
    public int scoreInt;
   

    // Start is called before the first frame update
    void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        UserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        playerScore = 0;

    }
   
    void Update()
    {
        scoreTexto.text = playerScore.ToString();
    }

    public void WriteNewScore(int score)
    {
        SetUserHighscore();
       /* UserData data = new UserData();

        data.score = int.Parse( GameObject.Find("InputScore").GetComponent<InputField>().text);
        string json = JsonUtility.ToJson(data);

        mDatabase.Child("users").Child(UserId).SetRawJsonValueAsync(json);*/ 

    }
    public void SetUserHighscore()
    {

        FirebaseDatabase.DefaultInstance
           .GetReference("users/" + UserId)
           .GetValueAsync().ContinueWithOnMainThread(task => {
               //Debug.Log("entrando al metodo" + task);
               if (task.IsFaulted)
               {
                   Debug.Log(task.Exception);
                   //Debug.Log("faulted");
               }
               else if (task.IsCompleted)
               {
                   try
                   {
                       //Debug.Log("completed");
                       DataSnapshot snapshot = task.Result;
                       Debug.Log("snapshot value: " + snapshot.Value);

                       var data = (Dictionary<string, object>)snapshot.Value;
                       //Debug.Log("data" + data["score"]);



                       highestScore = Convert.ToInt32(data["score"]);
                       var username = Convert.ToString(data["username"]);
                       //Debug.Log("Highscore: " + highestScore);
                       // Debug.Log("Playerscore: " + playerScore);
                       // Debug.Log("Username: " + username);

                       UserData update = new UserData();
                       if (playerScore >= highestScore)
                       {
                           update.score = playerScore;
                           update.username = username;
                           string json = JsonUtility.ToJson(update);

                           mDatabase.Child("users").Child(UserId).SetRawJsonValueAsync(json);
                           highestScore = playerScore;
                       }
                   }
                   catch (Exception e)
                   {
                       Debug.Log(e);
                   }

               }
           });
    }

   /* public void SetNewScore()
    {
        try
        {
            if ((int)playerScore < Score.score)
            {
                UserData data = new UserData();
                data.score = Score.score;
                data.username = UserId;
                Debug.Log(UserId + "     ======     " + playerScore + "     ======     " + Score.score);
                string json = JsonUtility.ToJson(data);
                mDatabase.Child("users").Child(UserId).SetRawJsonValueAsync(json);

            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
   */
    public void GetUserScore()
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("users/" + UserId)
            .GetValueAsync().ContinueWith(task => {
                if (task.IsFaulted)
                {
                    Debug.Log(task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    Debug.Log(snapshot.Value);

                    var data = (Dictionary<string, object>)snapshot.Value;
                    highestScore = (int)data["score"];

                    Debug.Log("Puntaje: " + data["score"]);

                    /*                    foreach (var userDoc in (Dictionary<string,object>) snapshot.Value)
                                        {
                                            Debug.Log(userDoc.Key);
                                            Debug.Log(userDoc.Value);
                                        }*/

                }
            });
    }

    public void GetLeaders()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("score").LimitToLast(5).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                //Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                //Debug.Log(snapshot);
                tablaLeadersPanel.SetActive(true);

                foreach (var userDoc in (Dictionary<string, object>)snapshot.Value)
                {

                    userObject = (Dictionary<string, object>)userDoc.Value;
                    //var orderedScoresList = userObject.Values.OrderByDescending(x => ((Dictionary<string, object>)x)["score"]);
                    Debug.Log(userObject["username"] + ":" + userObject["score"]);

                    leadersText.text += (userObject["username"] + ":" + userObject["score"] + Environment.NewLine);
                }

            }
        });
    }
    // Update is called once per frame
    public void ClosePanelLeaders()
    {
        tablaLeadersPanel.SetActive(false);
        leadersText.text = "";
    }

    public void ScoretoInt()
    {
        scoreInt = Convert.ToInt32(userObject["score"]);
        //Debug.Log("Score to Int: " + scoreInt);


    }
}

public class UserData
{
    public int score;
    public string username;
}
