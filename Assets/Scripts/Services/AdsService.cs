using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Services
{
    public class AdsService : Service
    {
        public async UniTask<bool> PlayVideo()
        {
            Debug.Log(nameof(PlayVideo));
            return true;
        }

        public async UniTask<bool> PlayInterstitial()
        {
            Debug.Log(nameof(PlayInterstitial));
            MaxSdk.ShowInterstitial(AdsInitializer.AdUnitId);
            return true;
        }

        public void ShowBanner()
        {
            Debug.Log(nameof(ShowBanner));
            MaxSdk.ShowBanner(AdsInitializer.BannerAdUnitId);
        }

        public override void Initialize()
        {
        }
    }
}