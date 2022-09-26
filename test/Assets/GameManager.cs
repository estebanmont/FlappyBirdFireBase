using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Linq;
using Firebase.Auth;

public class GameManager : MonoBehaviour
{
    DatabaseReference mmDatabase;
    public GameObject gameOverCanvas;
    public static int playerScore;
    private FB_ScoreController pScore;

    private Score score;
    public string _username;
    string UserId;
    public long highestScore;


    private void Start()
    {
        mmDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        Time.timeScale = 1;
        FirebaseAuth.DefaultInstance.StateChanged += GetUserScore;
        UserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        FirebaseAuth.DefaultInstance.StateChanged += GetName;
    }

    public void SetUserHighscore()
    {
        
            try
            {
                if ((int)highestScore< Score.playerScore)
                {
                    UserData data = new UserData();
                    data.score = Score.playerScore;
                    data.username = _username;
                    Debug.Log(highestScore);
                    Debug.Log(playerScore);
                    string json = JsonUtility.ToJson(data);
                    mmDatabase.Child("users").Child(UserId).SetRawJsonValueAsync(json);

                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        
    }

    private void GetName(object sender, EventArgs e)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("users/" + UserId + "/username")
            .GetValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsFaulted)
                {
                    Debug.Log(task.Exception);
                   
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    _username = (string)snapshot.Value;
                    
                }
            });

    }
    private void GetUserScore(object sender, EventArgs e)
    {

        FirebaseDatabase.DefaultInstance
        .GetReference("users/" + UserId + "/score")
        .GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log(snapshot);
                highestScore = (long)snapshot.Value;
                Debug.Log("HolaSoyScore" + highestScore);
            }
        });
    }

    // Start is called before the first frame update

    public void GameOver()
    {
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0;
        SetUserHighscore();





    }
    public void Replay()
    {
        SceneManager.LoadScene(0);
    }
}
