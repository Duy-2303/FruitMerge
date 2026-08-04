using System;
using GoogleMobileAds.Api;
using UnityEngine;

public class GoogleAdsManager : MonoBehaviour
{
    public static GoogleAdsManager Instance { get; private set; }
    public event Action<int> BannerHeightChanged;

    public int BannerHeightPixels { get; private set; }

#if UNITY_ANDROID
    // ID quảng cáo thật
    private const string ProductionBannerId =
        "ca-app-pub-6827192020553702/6486089666";

    private const string ProductionInterstitialId =
        "ca-app-pub-6827192020553702/7113755565";

    private const string ProductionRewardedId =
        "ca-app-pub-6827192020553702/6677661357";

    // ID test chính thức của Google
    private const string TestBannerId =
        "ca-app-pub-3940256099942544/6300978111";

    private const string TestInterstitialId =
        "ca-app-pub-3940256099942544/1033173712";

    private const string TestRewardedId =
        "ca-app-pub-3940256099942544/5224354917";
#endif

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    private bool isInterstitialLoading;
    private bool isRewardedLoading;

    private static string BannerId
    {
        get
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            return TestBannerId;
#else
            return ProductionBannerId;
#endif
        }
    }

    private static string InterstitialId
    {
        get
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            return TestInterstitialId;
#else
            return ProductionInterstitialId;
#endif
        }
    }

    private static string RewardedId
    {
        get
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            return TestRewardedId;
#else
            return ProductionRewardedId;
#endif
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
     
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        MobileAds.Initialize(_ =>
        {
            Debug.Log("Google Mobile Ads initialized.");
            ShowBanner();
            LoadRewarded();
            LoadInterstitial();
        });
    }

    #region Banner

    public void ShowBanner()
    {
        AdSize adaptiveSize =
    AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(
        AdSize.FullWidth);
        if (bannerView != null)
        {
            bannerView.Show();
            return;
        }

        bannerView = new BannerView(
            BannerId,
            adaptiveSize,
            AdPosition.Bottom);

        bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner loaded.");
            BannerHeightPixels = Mathf.CeilToInt(bannerView.GetHeightInPixels());
            BannerHeightChanged?.Invoke(BannerHeightPixels);
        };

        bannerView.OnBannerAdLoadFailed += error =>
        {
            Debug.LogWarning($"Banner load failed: {error}");
        };

        bannerView.LoadAd(new AdRequest());
    }

    public void HideBanner()
    {
        bannerView?.Hide();
        BannerHeightPixels = 0;
        BannerHeightChanged?.Invoke(0);
    }

    public void DestroyBanner()
    {
        bannerView?.Destroy();
        bannerView = null;
        BannerHeightPixels = 0;
        BannerHeightChanged?.Invoke(0);
    }

    #endregion

    #region Rewarded

    public void LoadRewarded()
    {
        if (isRewardedLoading)
            return;

        isRewardedLoading = true;

        rewardedAd?.Destroy();
        rewardedAd = null;

        RewardedAd.Load(
            RewardedId,
            new AdRequest(),
            (ad, error) =>
            {
                isRewardedLoading = false;

                if (error != null || ad == null)
                {
                    Debug.LogWarning(
                        $"Rewarded load failed: {error}");
                    return;
                }

                rewardedAd = ad;
                RegisterRewardedEvents(ad);

                Debug.Log("Rewarded loaded.");
            });
    }

    public bool IsRewardedReady()
    {
        return rewardedAd != null &&
               rewardedAd.CanShowAd();
    }

    public void ShowRewarded(Action onRewardGranted)
    {
        if (!IsRewardedReady())
        {
            Debug.LogWarning("Rewarded chưa sẵn sàng.");
            LoadRewarded();
            return;
        }

        RewardedAd currentAd = rewardedAd;
        rewardedAd = null;

        currentAd.Show(_ =>
        {
            // Chỉ cấp thưởng trong callback này.
            onRewardGranted?.Invoke();
        });
    }

    private void RegisterRewardedEvents(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            ad.Destroy();
            LoadRewarded();
        };

        ad.OnAdFullScreenContentFailed += error =>
        {
            Debug.LogWarning(
                $"Rewarded show failed: {error}");

            ad.Destroy();
            LoadRewarded();
        };
    }

    #endregion

    #region Interstitial

    public void LoadInterstitial()
    {
        if (isInterstitialLoading)
            return;

        isInterstitialLoading = true;

        interstitialAd?.Destroy();
        interstitialAd = null;

        InterstitialAd.Load(
            InterstitialId,
            new AdRequest(),
            (ad, error) =>
            {
                isInterstitialLoading = false;

                if (error != null || ad == null)
                {
                    Debug.LogWarning(
                        $"Interstitial load failed: {error}");
                    return;
                }

                interstitialAd = ad;

                Debug.Log("Interstitial loaded.");
            });
    }

    public void ShowInterstitial(Action onFinished)
    {
        if (interstitialAd == null ||
            !interstitialAd.CanShowAd())
        {
            Debug.Log("Interstitial chưa sẵn sàng.");

            LoadInterstitial();
            onFinished?.Invoke();
            return;
        }

        InterstitialAd currentAd = interstitialAd;
        interstitialAd = null;

        bool finished = false;

        void Finish()
        {
            if (finished)
                return;

            finished = true;

            currentAd.Destroy();
            LoadInterstitial();

            onFinished?.Invoke();
        }

        currentAd.OnAdFullScreenContentClosed += Finish;

        currentAd.OnAdFullScreenContentFailed += error =>
        {
            Debug.LogWarning(
                $"Interstitial show failed: {error}");

            Finish();
        };

        currentAd.Show();
    }

    #endregion

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        bannerView?.Destroy();
        interstitialAd?.Destroy();
        rewardedAd?.Destroy();
    }
}
