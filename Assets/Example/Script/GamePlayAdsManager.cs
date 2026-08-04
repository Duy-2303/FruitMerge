using UnityEngine;
using Link;
namespace DuyDZ.MergeFood
{
    public class GamePlayAdsManager : MonoBehaviour
    {
        private void Start()
        {
            GoogleAdsManager ads = GoogleAdsManager.Instance;
            if (ads == null)
                return;

            ads.BannerHeightChanged += ApplyBannerInset;
            ads.ShowBanner();
            ApplyBannerInset(ads.BannerHeightPixels);
        }

        private static void ApplyBannerInset(int heightPixels)
        {
            if (CameraControl.Instance != null)
                CameraControl.Instance.SetBottomInsetPixels(heightPixels);
        }

        private void OnDestroy()
        {
            GoogleAdsManager ads = GoogleAdsManager.Instance;
            if (ads != null)
            {
                ads.BannerHeightChanged -= ApplyBannerInset;
                ads.HideBanner();
            }

            ApplyBannerInset(0);
        }
    }
}

