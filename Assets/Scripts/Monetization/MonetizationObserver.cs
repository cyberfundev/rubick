using System;
using Level;
using Services;
using UniRx;
using Zenject;

namespace Monetization
{
    public class MonetizationObserver : IInitializable
    {
        private readonly AdsService _adsService;
        private readonly LevelLooper _levelLooper;
        private readonly MonetizationConfig _config;

        private int _interAttempts;

        private DateTime _lastInterTime;

        public MonetizationObserver(AdsService adsService, LevelLooper levelLooper, MonetizationConfig config)
        {
            _config = config;
            _levelLooper = levelLooper;
            _adsService = adsService;
        }

        public void Initialize()
        {
            _levelLooper.OnSideCompleted.Subscribe(OnSideCompleted);
            _levelLooper.OnShuffle.Subscribe(_ => ShowInterIfNeed());
            
            _adsService.ShowBanner();
        }

        private void OnSideCompleted(int sides)
        {
            ShowInterIfNeed();
        }

        private async void ShowInterIfNeed()
        {
            _interAttempts++;
            if(_config.SkipFirstIntersAmount >= _interAttempts)
                return;
            
            var timeSinceInter = DateTime.UtcNow - _lastInterTime;
            if (timeSinceInter.Seconds >= _config.InterstitialCoolDown)
            {
                bool success = await _adsService.PlayInterstitial();
                if (success)
                {
                    _lastInterTime = DateTime.UtcNow;
                }
            }
        }
    }
}
