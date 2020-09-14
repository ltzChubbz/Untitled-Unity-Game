using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    public IdleTutorialScript game;

    public RewardedAd rewardedAd1;

    public int timesFailed;

    public Button watchAd;

    public GameObject rewardPopUp;


    public void Start()
    {
#if UNITY_ANDROID
        string appID = "ca-app-pub-1877733232489239~2933926197";
#elif UNITY_IPHONE
        string appID = "unexpected_platform";
#else
        string appID = "unexpected_platform";
#endif
        MobileAds.Initialize(initStatus => { });
        //MobileAds.Initialize(appID);
        rewardPopUp.gameObject.SetActive(false);

    }

    public void Update()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        if (rewardedAd1.IsLoaded())
            watchAd.gameObject.SetActive(true);
        else
            watchAd.gameObject.SetActive(false);
#else
        watchAd.gameObject.SetActive(false);
#endif
    }

    public void CreateAndLoadRewardedAd()
    {
        rewardedAd1 = new RewardedAd("ca-app-pub-3940256099942544/5224354917"); //RewardedAd("ca-app-pub-1877733232489239/1429272835");

        rewardedAd1.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        rewardedAd1.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        rewardedAd1.OnUserEarnedReward += HandleUserEarnedReward;

        AdRequest request = new AdRequest.Builder().Build();
        rewardedAd1.LoadAd(request);
    }


    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        switch (timesFailed)
        {
            case 0:
                Invoke("CreateAndLoadRewardedAd()", 10);
                break;
            case 1:
                Invoke("CreateAndLoadRewardedAd()", 30);
                break;
            case 2:
                Invoke("CreateAndLoadRewardedAd()", 60);
                break;
            case 3:
                Invoke("CreateAndLoadRewardedAd()", 120);
                break;
            default:
                Invoke("CreateAndLoadRewardedAd()", 240);
                break;
        }
        timesFailed++;

    }
    
    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        CreateAndLoadRewardedAd();
    }
    
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        //Reward
        game.data.coins += args.Amount;

        //Create Ad
        CreateAndLoadRewardedAd();
    }

    public void WatchAd()
    {
        if (rewardedAd1.IsLoaded())
        {
            rewardedAd1.Show();
            watchAd.gameObject.SetActive(false);
            rewardPopUp.gameObject.SetActive(true);
        }
    }

    public void CloseAdReward()
    {
        rewardPopUp.gameObject.SetActive(false);
    }

}
