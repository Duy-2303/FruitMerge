using Link;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
namespace DuyDZ.MergeFood.Test
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Ins;
        int score;
        private void Awake()
        {
            Ins=this;
        }
        public void AddScore(int plusScore)
        {
            score += plusScore;
            Debug.Log("Score: " + score);
        }
    }
}