using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

using UnityEngine.Advertisements;
using System;

namespace MobileFramework
{
    public class AdManager : MonoBehaviour
    {
        #region Variables
        static AdManager instance;
        public delegate void RewardedAdClosed(bool rewarded);
        public event RewardedAdClosed OnRewardedAdClosed;
        public string AndroidAppID = "ca-app-pub-3940256099942544~3347511713";
        public string IOSAppID = "ca-app-pub-3940256099942544~1458002511";
        [SerializeField]
        private Banner[] Banners;
        [SerializeField]
        private Interstitial[] Interstitials;
        [SerializeField]
        private RewardedAd[] RewardedAds;
        public static AdManager Instance
        {
            get { return instance; }
        }
        #endregion
        #region Classes
        [System.Serializable]
        public class Ad
        {
            protected bool Destroyed;
            protected bool loaded;
            public bool Loaded
            {
                get { return loaded; }
            }
            protected bool MustShow;
            public string AndroidAppID = "ca-app-pub-3940256099942544~3347511713";
            public string IOSAppID = "ca-app-pub-3940256099942544~1458002511";
            public string adUnitId
            {
                get
                {
#if UNITY_ANDROID
                    return AndroidAppID;
#elif UNITY_IPHONE
            return IOSAppID;
#else
                    return "unexpected_platform";
#endif
                }
            }
            public virtual void TryShow()
            {
                MustShow = false;
                if (Loaded)
                {
                    Show();
                }
                else
                {
                    MustShow = true;
                }
            }
            public virtual void Show()
            {

            }
            public virtual void Hide()
            {
                MustShow = false;
            }
            public virtual void Destroy()
            {
                MustShow = false;
                Destroyed = true;
            }
            public virtual void TryLoad()
            {
             
            }
            public virtual void LoadFailed()
            {
                InternetManager.Instance.OnInternetRecoverConnection += Load;
                InternetManager.Instance.RequestUpdate();
            }
            public virtual void Load()
            {
                Destroyed = false;
                InternetManager.Instance.OnInternetRecoverConnection -= Load;
            }
            protected virtual void HandleOnAdLoaded(object sender, EventArgs args)
            {
                loaded = true;
                //if (MustShow)
                //{
                //    TryShow();
                //}
            }
            protected virtual void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
            {
                if (InternetManager.Instance.IsInternetAvailable)
                {
                    Destroy();
                    TryLoad();
                }
                else
                {
                    Destroy();
                    InternetManager.Instance.OnInternetRecoverConnection += TryLoad;
                }
            }
        }
        [System.Serializable]
        public class Banner : Ad
        {
            public AdPosition adPosition = AdPosition.Bottom;
            public Vector2Int bannerSize = new Vector2Int(320, 50);
            private BannerView view;
            public override void Destroy()
            {
                base.Destroy();
                view.Destroy();
            }
            public override void Hide()
            {
                base.Hide();
                view.Hide();
            }
            public override void TryLoad()
            {
                base.TryLoad();
                view = new BannerView(adUnitId, AdSize.Banner, adPosition);
                view.OnAdLoaded += HandleOnAdLoaded;
                view.OnAdFailedToLoad += HandleOnAdFailedToLoad;
                if (InternetManager.Instance.IsInternetAvailable)
                {
                    Load();
                }
                else
                {
                    LoadFailed();
                }
            }
            public override void Load()
            {
                base.Load();
                AdRequest request = new AdRequest.Builder().Build();
                view.LoadAd(request);
            }
            public override void Show()
            {
                view.Show();
            }
        }
        [System.Serializable]
        public class Interstitial : Ad
        {
            private InterstitialAd ad;
            public new bool Loaded
            {
                get { return ad.IsLoaded(); }
            }
            public override void Show()
            {
                ad.Show();
            }
            public override void Destroy()
            {
                base.Destroy();
                ad.Destroy();
            }
            public override void TryLoad()
            {
                base.TryLoad();
                ad = new InterstitialAd(adUnitId);
                ad.OnAdLoaded += HandleOnAdLoaded;
                ad.OnAdFailedToLoad += HandleOnAdFailedToLoad;
                ad.OnAdClosed += HandleRewardBasedVideoClosed;
                if (InternetManager.Instance.IsInternetAvailable)
                {
                    Load();
                }
                else
                {
                    LoadFailed();
                }
            }
            public override void Load()
            {
                base.Load();
                AdRequest request = new AdRequest.Builder().Build();
                ad.LoadAd(request);
            }
            public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
            {
                TryLoad();
            }
        }
        [System.Serializable]
        public class RewardedAd : Ad
        {
            private RewardBasedVideoAd ad;
            bool rewarded;
            bool closed;
            public bool Closed
            {
                get { return closed; }
            }
            public bool Rewarded
            {
                get { return rewarded; }
            }
            public new bool Loaded
            {
                get { return ad.IsLoaded(); }
            }
            public override void Show()
            {
                closed = false;
                rewarded = false;
                ad.Show();
            }
            public override void Destroy()
            {
                base.Destroy();
            }

            public override void TryLoad()
            {
                base.TryLoad();
                ad = RewardBasedVideoAd.Instance;
                ad.OnAdLoaded += HandleOnAdLoaded;
                ad.OnAdFailedToLoad += HandleOnAdFailedToLoad;
                ad.OnAdRewarded += HandleRewardBasedVideoRewarded;
                ad.OnAdClosed += HandleRewardBasedVideoClosed;
                if (InternetManager.Instance.IsInternetAvailable)
                {
                    Load();
                }
                else
                {
                    LoadFailed();
                }
            }
            public override void Load()
            {
                base.Load();
                AdRequest request = new AdRequest.Builder().AddTestDevice(SystemInfo.deviceUniqueIdentifier).Build();
                ad.LoadAd(request, adUnitId);
            }
            public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
            {
                closed = true;
                TryLoad();
                AdManager.instance.CloseRewardedAds(rewarded);
            }
            public void HandleRewardBasedVideoRewarded(object sender, Reward args)
            {
                rewarded = true;
            }
        }
        #endregion
        #region Mono
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(this);
            instance = this;
            Init();
        }
        public void Init()
        {
#if UNITY_ANDROID
                string appId = AndroidAppID;
#elif UNITY_IPHONE
            string appId = IOSAppID;
#else
            string appId = "unexpected_platform";
#endif
                MobileAds.Initialize(appId);
                RequestBanners();
                RequestInterstitials();
                RequestRewardedAds();
        }
        #endregion
        #region Requests
        void RequestBanners()
        {
            for (int i = 0; i < Banners.Length; i++)
            {
                LoadBanner(i);
            }
        }

        private void RequestInterstitials()
        {
            for (int i = 0; i < Interstitials.Length; i++)
            {
                LoadInterstitial(i);
            }
        }

        private void RequestRewardedAds()
        {
            for (int i = 0; i < RewardedAds.Length; i++)
            {
                LoadRewardedAds(i);
            }
        }
        #endregion
        #region Banners

        public void LoadBanner(int index)
        {
            if (index < Banners.Length)
            {
                Banners[index].TryLoad();
            }
        }

        public void ShowBanner(int index)
        {
            if (index < Banners.Length)
            {
                Banners[index].Show();
            }
        }

        public void HideBanner(int index)
        {
            if (index < Banners.Length)
            {
                Banners[index].Hide();
            }
        }

        public void DestroyBanner(int index)
        {
            if (index < Banners.Length)
            {
                Banners[index].Destroy();
            }
        }

        #endregion
        #region Interstitials

        public void LoadInterstitial(int index)
        {
            if (index < Interstitials.Length)
            {
                Interstitials[index].TryLoad();
            }
        }

        public void ShowInterstitial(int index)
        {
            if (index < Interstitials.Length)
            {
                Interstitials[index].Show();
            }
        }

        public void HideInterstitial(int index)
        {
            if (index < Interstitials.Length)
            {
                Interstitials[index].Hide();
            }
        }

        public void DestroyInterstitial(int index)
        {
            if (index < Interstitials.Length)
            {
                Interstitials[index].Destroy();
            }
        }

        #endregion
        #region RewardedAds
        public void LoadRewardedAds(int index)
        {
            if (index < RewardedAds.Length)
            {
                RewardedAds[index].TryLoad();
            }
        }
        public void ShowRewardedAds(int index)
        {
            Debug.LogError("Try to show");
            if (index < RewardedAds.Length)
            {
                RewardedAds[index].Show();
            }
        }
        public void HideRewardedAds(int index)
        {
            if (index < RewardedAds.Length)
            {
                RewardedAds[index].Hide();
            }
        }
        public void DestroyRewardedAds(int index)
        {
            if (index < RewardedAds.Length)
            {
                RewardedAds[index].Destroy();
            }
        }
        public bool IsRewardedAdLoaded(int index)
        {
            if (index < RewardedAds.Length)
            {
                return RewardedAds[index].Loaded;
            }
            return false;
        }
        public bool IsRewardedAdClosed(int index)
        {
            if (index < RewardedAds.Length)
            {
                return RewardedAds[index].Closed;
            }
            return false;
        }
        public bool IsRewardedAdRewarded(int index)
        {
            if (index < RewardedAds.Length)
            {
                return RewardedAds[index].Rewarded;
            }
            return false;
        }

        #endregion
        #region Events 
        public void CloseRewardedAds(bool rewarded)
        {
            OnRewardedAdClosed(rewarded);
        }
        #endregion
    }
}