using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DuyDZ.MergeFood.Test;

namespace DuyDZ.MergeFood
{
    public class EndGamePopUpUi : MonoBehaviour
    {
        [SerializeField] TMP_Text scoreText;
        [SerializeField] TMP_Text bestScoreText;
        [SerializeField] Button replayButton;
        [SerializeField] Button homeButton;

        private void Awake()
        {
            replayButton.onClick.AddListener(Replay);
            homeButton.onClick.AddListener(GoHome);
        }
        public void Show()
        {
    
            //stop physic,time, overrall stop all the game
            Time.timeScale = 0;
            GoogleAdsManager ads = GoogleAdsManager.Instance;
            if(ads == null)
            {
                ShowPopUpContent();
                return;
            }
            ads.ShowInterstitial(ShowPopUpContent);
        }
        void ShowPopUpContent()
        {
            ScoreManager scoreManager = ScoreManager.GetOrCreate();
            scoreText.text = scoreManager.CurrentScore.ToString();
            bestScoreText.text = scoreManager.BestScore.ToString();
            transform.SetAsLastSibling();
            gameObject.SetActive(true);
        }
        private void GoHome()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("HomeScene");
        }

        private void Replay()
        {
            Time.timeScale = 1;
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }
    }
}
