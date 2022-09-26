using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddScore : MonoBehaviour
{
    //public int pScore;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Score.playerScore++;
       // pScore = FB_ScoreController.playerScore;
    }

}
