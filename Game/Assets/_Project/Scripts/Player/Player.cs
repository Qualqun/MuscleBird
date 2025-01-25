using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    #region Fields
    private int score = 0;

    public enum PlayerID { Player1, Player2 }
    public PlayerID playerID;
    [SerializeField] Text scoreUI;

    private TMP_Text scoreText;

    //Events
    public event Action<Player> turnHasEnded;

    private bool constructionFinished;

    //Properties
    public int Score => score;
    public bool ConstructionFinished {  get => constructionFinished; set => constructionFinished = value; }

    public TMP_Text ScoreText { get => scoreText; set => scoreText = value; }
    #endregion

    #region PublicMethodsd
   
    public void EndTurn()
    {
        turnHasEnded?.Invoke(this);
    }

    public void AddScore(int newScore)
    {
        score += newScore;
        // scoreUI.text = "Score : " + score.ToString();
        scoreText.text = $"Score : {score.ToString()}";
    }

    public void RemoveScore(int newScore)
    {
        score -= newScore;
        // scoreUI.text = "Score : " + score.ToString();
        scoreText.text = $"Score : {score.ToString()}";
    }
    #endregion
}
