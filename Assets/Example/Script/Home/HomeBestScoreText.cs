using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DuyDZ.MergeFood
{
    public class HomeBestScoreText : MonoBehaviour
    {
        const string bestScoreKey = "FruitMergeBestScore";
        [SerializeField] TMP_Text bestScoreText;
        private void Awake()
        {
            if(bestScoreText == null) 
                bestScoreText = GetComponent<TMP_Text>();
            bestScoreText.text = PlayerPrefs.GetInt(bestScoreKey, 0).ToString();
        }
    }
}