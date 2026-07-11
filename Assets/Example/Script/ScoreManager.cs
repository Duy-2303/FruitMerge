using Link;
using Sirenix.OdinInspector;
using System.Collections;
using System;
using TMPro;
using UnityEngine;
namespace DuyDZ.MergeFood.Test
{
    public class ScoreManager : MonoBehaviour
    {
        private const string BestScoreKey = "FruitMergeBestScore";

        public static ScoreManager Ins;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text bestScoreText;

        int score;
        int bestScore;

        public int CurrentScore => score;
        public int BestScore => bestScore;
        public event Action<int, int> OnScoreChanged;

        private void Awake()
        {
            Ins = this;
            bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
            UpdateScoreText();
        }

        public static ScoreManager GetOrCreate()
        {
            if (Ins != null)
                return Ins;

            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
            if (scoreManager != null)
                return scoreManager;

            GameObject holder = GameObject.Find("ScoreManager");
            if (holder == null)
                holder = new GameObject("ScoreManager");

            return holder.AddComponent<ScoreManager>();
        }

        public void AddScore(int plusScore)
        {
            score += plusScore;
            if (score > bestScore)
            {
                bestScore = score;
                PlayerPrefs.SetInt(BestScoreKey, bestScore);
                PlayerPrefs.Save();
            }

            Debug.Log("Score: " + score);
            UpdateScoreText();
        }

        public void BindScoreTexts(TMP_Text currentScoreText, TMP_Text topBestScoreText)
        {
            scoreText = currentScoreText;
            bestScoreText = topBestScoreText;
            UpdateScoreText();
        }

        private void UpdateScoreText()
        {
            if (scoreText != null)
                scoreText.text = score.ToString();

            if (bestScoreText != null)
                bestScoreText.text = bestScore.ToString();

            OnScoreChanged?.Invoke(score, bestScore);
        }
    }
}
