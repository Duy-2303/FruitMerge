using UnityEngine;
namespace DuyDZ.MergeFood
{
    public class GamePlayAdsManager : MonoBehaviour
    {
        private void Start()
        {
            GoogleAdsManager.Instance?.ShowBanner();
        }

        private void OnDestroy()
        {
            GoogleAdsManager.Instance?.HideBanner();
        }
    }
}

