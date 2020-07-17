using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.UI;

public class Admob : SingletonMonoBehaviour<Admob>
{

    private BannerView bannerView;

    private InterstitialAd interstitial;
    private bool is_close_interstitial = false;
    private int reShowCount_Interstitial = 0; //ロードできなかった時のカウント変数

    private RewardedAd rewardedAd;
    private int reShowCount_Rewarded = 0; //ロードできなかった時のカウント変数


    //＝＝＝＝＝＝＝＝＝＝＝画面の向き検知用＝＝＝＝＝＝＝＝＝＝＝＝

    private bool canCheckPortrait = false;
    private bool canCheckLandscape = false;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID
        //アプリID
        string androidId = "ca-app-pub-1961512879550106~2117447901";
#elif UNITY_IPHONE
      string adUnitId = "toDo";
#else
      string adUnitId = "unexpected_platform";
#endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(androidId);

        switch (GameStateManager.Instance.StateProperty)
        {
            case GameStateManager.GameState.Title:
            case GameStateManager.GameState.Battle_Single:
                canCheckPortrait = true;
                break;
            case GameStateManager.GameState.Battle_Multi:
                canCheckLandscape = true;
                break;
        }

    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private void Update()
    {

        if (canCheckPortrait)
        {
            if((GameStateManager.Instance.StateProperty == GameStateManager.GameState.Battle_Single 
                || GameStateManager.Instance.StateProperty == GameStateManager.GameState.Title) && 
                (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown))
            {
                AdReadyFunc();
                canCheckPortrait = false;
            }
        }


        if (canCheckLandscape)
        {
            if (GameStateManager.Instance.StateProperty == GameStateManager.GameState.Battle_Multi && 
                (Screen.orientation == ScreenOrientation.Landscape || Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight))
            {
                AdReadyFunc();
                canCheckLandscape = false;
            }
        }
}
#endif

    public void AdReadyFunc()
    {
        switch (GameStateManager.Instance.StateProperty)
        {
            case GameStateManager.GameState.Title:

                RequestBanner(); //バナー読み込み
                RequestInterstitial(); //インタースティシャル広告読み込み

                break;

            case GameStateManager.GameState.Battle_Single:

                RequestBanner(); //バナー読み込み
                RequestInterstitial(); //インタースティシャル広告読み込み
                RequestRewardedAd(); //リワード広告読み込み

                break;

            case GameStateManager.GameState.Battle_Multi:

                RequestBanner(); //バナー読み込み
                RequestInterstitial(); //インタースティシャル広告読み込み

                break;

            default:

                Debug.Log("そんなステートはない");

                break;
        }
    }


    //＝＝＝＝＝＝＝＝＝＝＝＝バナー広告＝＝＝＝＝＝＝＝＝＝＝

    public void RequestBanner()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            Debug.Log("ad:バナー広告作成前に既にあるBannerViewを破棄する");
        }
        else if (bannerView == null)
        {
            Debug.Log("ad:バナー広告作成前にBannerViewは存在しない");
        }

        // 広告ユニットID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111"; //テストID

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Top);

        AdRequest adRequest = new AdRequest.Builder().Build();

        bannerView.LoadAd(adRequest);

    }

    public void DestroyBanner()
    {
        if(bannerView != null)
        {
            bannerView.Destroy();
            Debug.Log("ad:バナー広告作成前に既にあるBannerViewを破棄する");
        }
        else
        {
            Debug.Log("ad:バナー広告作成前にBannerViewは存在しない");
        }
    }



    //＝＝＝＝＝＝＝＝＝＝＝＝インタースティシャル広告＝＝＝＝＝＝＝＝＝＝＝

    public void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712"; //テストID
#elif UNITY_IPHONE
    string adUnitId = ios_Interstitial;
#else
    string adUnitId = "unexpected_platform";
#endif

        if (is_close_interstitial == true)
        {
            interstitial.Destroy();
        }

        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(adUnitId);

        // Called when an ad request has successfully loaded.
        interstitial.OnAdLoaded += HandleOnInterstitialAdLoaded;

        // Called when an ad request failed to load.
        interstitial.OnAdFailedToLoad += HandleOnInterstitialAdFailedToLoad;

        // Called when an ad is shown.
        interstitial.OnAdOpening += HandleOnInterstitialAdOpened;

        // Called when the ad is closed.
        interstitial.OnAdClosed += HandleOnInterstitialAdClosed;


        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        interstitial.LoadAd(request);
        is_close_interstitial = false;　//再生フラグ無効化
    }


    public void HandleOnInterstitialAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("インタースティシャル広告読み込み完了");
    }

    public void HandleOnInterstitialAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("インタースティシャル広告読み込み失敗");
    }

    public void HandleOnInterstitialAdOpened(object sender, EventArgs args)
    {
        Debug.Log("インタースティシャル広告再生");
    }

    public void HandleOnInterstitialAdClosed(object sender, EventArgs args)
    {
        is_close_interstitial = true; //再生フラグ有効化
        Debug.Log("インタースティシャル広告終了");
    }

    public void ShowInterstitial()
    {
        if (interstitial == null)
        {
            RequestInterstitial();
        }

        if (interstitial.IsLoaded())
        {
            interstitial.Show();
            reShowCount_Interstitial = 0;
        }
        else
        {
            //準備できてなかったら0.1秒ごとに準備できてるか確認
            if (reShowCount_Interstitial < 10)
            {
                Invoke("ShowInterstitial", 0.1f);
                reShowCount_Interstitial++;
            }
            else
            {
                //1秒たっても準備できなかったとき
                reShowCount_Interstitial = 0;
            }
        }
    }



    //＝＝＝＝＝＝＝＝＝＝＝＝リワード広告＝＝＝＝＝＝＝＝＝＝＝

    public void RequestRewardedAd()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/5224354917"; //テストID
#elif UNITY_IPHONE
    string adUnitId = ios_Interstitial;
#else
    string adUnitId = "unexpected_platform";
#endif


        rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;


        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }


    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("リワード広告読み込み完了");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        Debug.Log("リワード広告読み込み失敗");
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        Debug.Log("リワード広告再生中");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        Debug.Log("リワード広告再生失敗");
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        RequestRewardedAd();
        Debug.Log("リワード広告再生完了");
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        Debug.Log("リワード広告の報酬提供");
    }



    public void UserChoseToWatchAd()
    {
        if (rewardedAd.IsLoaded())
        {
            rewardedAd.Show();
        }
        else
        {
            //準備できてなかったら0.1秒ごとに準備できてるか確認
            if (reShowCount_Rewarded < 10)
            {
                Invoke("UserChoseToWatchAd", 0.1f);
                reShowCount_Rewarded++;
            }
            else
            {
                //1秒たっても準備できなかったとき
                reShowCount_Rewarded = 0;
            }
        }
    }
}
